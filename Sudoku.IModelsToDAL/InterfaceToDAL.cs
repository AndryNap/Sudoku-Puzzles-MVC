using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.IModelsToDAL
{
    public interface InterfaceToDAL : IDisposable
    {
        Board GetBoard(int ProfileId);

        Board SetValue(int i, int j, int value, int ProfileId);

        bool Verify(int ProfileId);

        Board ShowSolution(int ProfileId);

        Board Recovery(int ProfileId);

        Board UndoStep(int ProfileId);

        Board NewGame(int ProfileId);


        Board OpenCell(int i, int j, int ProfileId);


        Board ToBegin(int ProfileId);


    }
}
