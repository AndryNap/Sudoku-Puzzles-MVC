using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sudoku.IModelsToDAL;
using Newtonsoft.Json;

namespace Sudoku.DALService
{
    public sealed class MainSudoku
    {
        private static MainSudoku instance;

        private static object OneOperation = new object();

        private static int[,] SolutionBoard;

        private static Profile[] ProfileArray;

        private static int SolutionNumber = 0;

        private const int MaxProfile = 5;

        private static IEnumerable<int> IEallowed;

        private static Cells[,] SingleBoard;

        private static Changer[] Chang;


        private MainSudoku() { }

        public static MainSudoku Instance
        {
            get
            {
                if (instance == null)
                {
                    lock(OneOperation)
                    {
                        instance = new MainSudoku();
                        SingleBoard = new Cells[9,9];                      
                        SolutionBoard = new int[9,9];
                        ProfileArray = new Profile[MaxProfile];
                        IEallowed = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsEnumerable<int>();
                        Chang = new Changer[10] { new Changer { i1 = 0, i2 = 0 }, new Changer { i1 = 0, i2 = 1 }, new Changer { i1 = 0, i2 = 2 }, new Changer { i1 = 1, i2 = 2 },
                                                  new Changer { i1 = 3, i2 = 4 }, new Changer { i1 = 3, i2 = 5 }, new Changer { i1 = 4, i2 = 5 },
                                                  new Changer { i1 = 6, i2 = 7 }, new Changer { i1 = 6, i2 = 8 }, new Changer { i1 = 7, i2 = 8 }
                                                };
                        Startup();
                    }
                }
                return instance;
            }
        }

        private static int GetSolutinNumber()
        {
            return Interlocked.Increment(ref SolutionNumber);
        }
        
        private static Board BoardSeed(Cells[,] SourceBoard, int Variant)
        {
            Board Seed = new Board();
            Seed.SBoard = new Cells[9, 9];
            Array.Copy(SourceBoard, Seed.SBoard, 81);
            Seed.SetMessage("");
            Seed.SetVariant(Variant);
            return Seed;
        }
        
       
        private static string BoardToJson(Board EnyBoard)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(EnyBoard);
        }

        private static Board BoardFromJson(string jsonboard)
        {
            return JsonConvert.DeserializeObject<Board>(jsonboard);
        }

        private static void Startup()
        {
            for (int p = 0; p < MaxProfile; p++)
            {
                int SolNumber = GetSolutinNumber();

                CreateSolution();
                InitialStartedValues();

                ProfileArray[p] = new Profile();

                ProfileArray[p].PlayBoard = BoardSeed(SingleBoard, SolNumber);

                ProfileArray[p].SolutionBoard = new int[9, 9];
                Array.Copy(SolutionBoard, ProfileArray[p].SolutionBoard,81);            
                ProfileArray[p].ProfileStack = new Stack<string>();

                ProfileArray[p].SolutionNumber = SolNumber;

                OpenCells(23, p);
               
            }           
        }

        public bool Verify(int ProfileId)
        {
            bool verified = true;
            lock(OneOperation)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (ProfileArray[ProfileId].PlayBoard.SBoard[i, j].value > 0)
                        {
                            if (ProfileArray[ProfileId].PlayBoard.SBoard[i, j].value != ProfileArray[ProfileId].SolutionBoard[i, j])
                            {
                                verified = false;
                            }
                        }
                    }
                }
            }
            return verified;
        }

        public Board ShowSolution(int ProfileId)
        {
            lock(OneOperation)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        ProfileArray[ProfileId].PlayBoard.SBoard[i, j].value = ProfileArray[ProfileId].SolutionBoard[i, j];
                        ProfileArray[ProfileId].PlayBoard.SBoard[i, j].given = true;
                    }
                }
                ProfileArray[ProfileId].PlayBoard.SetMessage("");
                return ProfileArray[ProfileId].PlayBoard;
            }
        }

        public Board Recovery(int ProfileId)
        {        
            lock (OneOperation)
            {              
                ProfileArray[ProfileId].PlayBoard = BoardFromJson(ProfileArray[ProfileId].ProfileStack.Peek());
                ProfileArray[ProfileId].PlayBoard.SetMessage("Игра восстановлена");
                return ProfileArray[ProfileId].PlayBoard;
            }
        }

        public Board UndoStep(int ProfileId)
        {
            lock (OneOperation)
            {
                if (ProfileArray[ProfileId].ProfileStack.Count > 1)
                {
                    ProfileArray[ProfileId].ProfileStack.Pop();
                    ProfileArray[ProfileId].PlayBoard = BoardFromJson(ProfileArray[ProfileId].ProfileStack.Peek());
                    ProfileArray[ProfileId].PlayBoard.SetMessage("");
                }
                else
                {
                    ProfileArray[ProfileId].PlayBoard = BoardFromJson(ProfileArray[ProfileId].ProfileStack.Peek());
                    ProfileArray[ProfileId].PlayBoard.SetMessage("Вы на стартовой позиции");
                }

                return ProfileArray[ProfileId].PlayBoard;
            }
        }

        public Board NewGame(int ProfileId)
        {
            lock (OneOperation)
            {
               int SolNumber = GetSolutinNumber();

                CreateSolution();
                InitialStartedValues();

                ProfileArray[ProfileId].PlayBoard = BoardSeed(SingleBoard, SolNumber);               

                ProfileArray[ProfileId].SolutionBoard = new int[9, 9];
                Array.Copy(SolutionBoard, ProfileArray[ProfileId].SolutionBoard, 81);
                ProfileArray[ProfileId].SolutionNumber = SolNumber;

                ProfileArray[ProfileId].ProfileStack.Clear();
                OpenCells(23, ProfileId);

                return ProfileArray[ProfileId].PlayBoard;
            }    
        }

       
        public Board ToBegin(int ProfileId)
        {
            lock(OneOperation)
            {
                while (ProfileArray[ProfileId].ProfileStack.Count > 1)
                {
                    ProfileArray[ProfileId].ProfileStack.Pop();
                }
                ProfileArray[ProfileId].PlayBoard = BoardFromJson(ProfileArray[ProfileId].ProfileStack.Peek());
                ProfileArray[ProfileId].PlayBoard.SetMessage("Вы на стартовой позиции");
                return ProfileArray[ProfileId].PlayBoard;
            }
        }
        private static void OpenCells(int n, int ProfileId )
        {
            Random rand1 = new Random(Guid.NewGuid().GetHashCode());
            Random rand2 = new Random(Guid.NewGuid().GetHashCode());
            int nCount = 0;
            int Cycles = 0;
            while ((nCount < n) && (Cycles < 300))
            {
                int rnd1 = rand1.Next(0, 9), rnd2 = rand2.Next(0, 9);
                if (ProfileArray[ProfileId].PlayBoard.SBoard[rnd1, rnd2].value == 0 ) 
                {
                    int value = ProfileArray[ProfileId].SolutionBoard[rnd1, rnd2];
                    if (SetValueInProfileStat(rnd1, rnd2, value, ProfileId))
                    {
                        ProfileArray[ProfileId].PlayBoard.SBoard[rnd1, rnd2].given = true;

                        nCount++;
                    }
                }
                Cycles++;
            }
            if (nCount > 0)
            {
               
                ProfileArray[ProfileId].ProfileStack.Push(BoardToJson(ProfileArray[ProfileId].PlayBoard));
            }
        }

        private static void InitialStartedValues()
        {             
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        SingleBoard[i, j] = new Cells { value = 0, given = false, allow = IEallowed, i = i, j = j };
                    }
                }
        }

        public Board GetBoard(int ProfileId)
        {
            lock(OneOperation)
            {
                return ProfileArray[ProfileId].PlayBoard;
            }
        }

        public Board SetValueInProfile(int i, int j, int value, int ProfileId)
        {
           
            lock (OneOperation)
            {
                if (ProfileArray[ProfileId].PlayBoard.SBoard[i, j].value == 0)
                {
                   
                    if (SetValueInProfileStat(i, j, value, ProfileId))
                    {
                        ProfileArray[ProfileId].ProfileStack.Push( BoardToJson(ProfileArray[ProfileId].PlayBoard));
                        if (ProfileArray[ProfileId].PlayBoard.StepCount == 81)
                        {
                            ProfileArray[ProfileId].PlayBoard.SetMessage("Поздравляю Вас! Вы правильно решили Судоку");
                        }
                        else
                        {
                            ProfileArray[ProfileId].PlayBoard.SetMessage("");
                        }
                        
                    }
                }
               

                return ProfileArray[ProfileId].PlayBoard;
            }
            
        }

        public Board OpenCell(int i, int j, int ProfileId )
        {
            lock(OneOperation)
            {
                if (valid(i, j, 1))
                {
                    int value = ProfileArray[ProfileId].SolutionBoard[i, j];
                    if (SetValueInProfileStat(i, j, value, ProfileId))
                    {
                        ProfileArray[ProfileId].PlayBoard.SBoard[i, j].given = true;
                        ProfileArray[ProfileId].ProfileStack.Push(BoardToJson(ProfileArray[ProfileId].PlayBoard));
                        ProfileArray[ProfileId].PlayBoard.SetMessage("");
                    }
                }
                return ProfileArray[ProfileId].PlayBoard;
            }
        }

        private static bool SetValueInProfileStat(int i, int j, int value, int ProfileId)
        {
            bool result = false;
            
                if (valid(i, j, value))
                {
                    ProfileArray[ProfileId].PlayBoard.SBoard[i, j].value = value;
                   
                    RemoveAllowedInProfile(i, j, value,  ProfileId);

                    ProfileArray[ProfileId].PlayBoard.Step();

                    result = true;
                }               
            
            return result;
        }

        private static bool  valid(int i, int j, int value)
        {
            bool result = false;
            if ((i >= 0) && (i < 9) && (j >= 0) && (j < 9) && (value >= 1) && (value <= 9)) { result = true; }
            
            return result;
        }

        private static bool CalcParam_3x3 (int ij, out int start, out int finish)
        {
            if ((ij >= 0) && (ij < 3))
            {
                start = 0;
                finish = 3;
                return true;
            }
            else if((ij >= 3) && (ij < 6))
            {
                start = 3;
                finish = 6;
                return true;
            }
            else if((ij >= 6) && (ij < 9))
            {
                start = 6;
                finish = 9;
                return true;
            }
            else
            {
                start = 0;
                finish = 0;
                return false;
            }    
        }
  
        private static void RemoveAllowedInProfile(int i, int j, int value, int ProfileId)
        {
            for (int ij = 0; ij < 9; ij++)
            {
                ProfileArray[ProfileId].PlayBoard.SBoard[ij, j].allow = ProfileArray[ProfileId].PlayBoard.SBoard[ij, j].allow.Where(w => w != value);
                ProfileArray[ProfileId].PlayBoard.SBoard[i, ij].allow = ProfileArray[ProfileId].PlayBoard.SBoard[i, ij].allow.Where(w => w != value);
            }

            int starti = 0, finishi = 0, startj = 0, finishj = 0;

            CalcParam_3x3(i, out starti, out finishi);
            CalcParam_3x3(j, out startj, out finishj);

            for (int ii = starti; ii < finishi; ii++)
            {
                for (int jj = startj; jj < finishj; jj++)
                {
                    ProfileArray[ProfileId].PlayBoard.SBoard[ii, jj].allow = ProfileArray[ProfileId].PlayBoard.SBoard[ii, jj].allow.Where(w => w != value);
                }
            }
        }

        private static void InitSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    SolutionBoard[i, j] = (i * 3 + i / 3 + j) % 9 + 1;
                }
            }
        }

        private static void ChangeTwoCell(int findValue1, int findValue2)
        {
            int xParam1, yParam1, xParam2, yParam2;
            xParam1 = yParam1 = xParam2 = yParam2 = 0;

            for (int i = 0; i < 9; i += 3)
            {
                for (int k = 0; k < 9; k += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            if (SolutionBoard[i + j, k + z] == findValue1)
                            {
                                xParam1 = i + j;
                                yParam1 = k + z;
                            }
                            if (SolutionBoard[i + j, k + z] == findValue2)
                            {
                                xParam2 = i + j;
                                yParam2 = k + z;
                            }

                        }
                    }

                    SolutionBoard[xParam1, yParam1] = findValue2;
                    SolutionBoard[xParam2, yParam2] = findValue1;
                }
            }
        }
        private static void UpdateSolution(int shuffleLevel)
        {
            for (int repeat = 0; repeat < shuffleLevel; repeat++)
            {
                Random rand1 = new Random(Guid.NewGuid().GetHashCode());
                Random rand2 = new Random(Guid.NewGuid().GetHashCode());
                ChangeTwoCell(rand1.Next(1, 10), rand2.Next(1, 10));
            }
        }

        private static void CreateSolution()
        {
            Random rand1 = new Random(Guid.NewGuid().GetHashCode());
            Random rand2 = new Random(Guid.NewGuid().GetHashCode());
            InitSolution();
            UpdateSolution(10);
            ChangerMix(rand1.Next(0, 33), rand2.Next(0, 33));           
            
        }

        public int checkProfileId(int ProfileId)
        {
            lock(OneOperation)
            {
                if ((ProfileId > 0) && (ProfileId <= MaxProfile))
                {
                    return ProfileId - 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        private static void ChangerMix(int level1, int level2)
        {
            Random rand1 = new Random(Guid.NewGuid().GetHashCode());
            Random rand2 = new Random(Guid.NewGuid().GetHashCode());

            int level_count = 0;

            while (level_count < level1)
            {
                Changer j_chang = Chang[rand1.Next(0, 10)];


                for (int i = 0; i < 9; i++)
                {
                    int i1 = SolutionBoard[i, j_chang.i1];
                    int i2 = SolutionBoard[i, j_chang.i2];
                    SolutionBoard[i, j_chang.i1] = i2;
                    SolutionBoard[i, j_chang.i2] = i1;

                }
                level_count++;
            }
            level_count = 0;
            while (level_count < level2)
            { 
                Changer i_chang = Chang[rand1.Next(0, 10)];

                for (int j = 0; j < 9; j++)
                {
                    int i1 = SolutionBoard[i_chang.i1, j];
                    int i2 = SolutionBoard[i_chang.i2, j];
                    SolutionBoard[i_chang.i1, j] = i2;
                    SolutionBoard[i_chang.i2, j] = i1;

                }

                level_count++;
            }


        }

        private class Profile
        {
            public int SolutionNumber { get; set; }
            public Board PlayBoard { get; set; }
            public int[,] SolutionBoard { get; set; }
            public Stack<string> ProfileStack { get; set; }

        }

        private class Changer
        {
            public int i1 { get; set; }
            public int i2 { get; set; }
        }

    }
}
