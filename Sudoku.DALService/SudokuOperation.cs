using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.IModelsToDAL;

namespace Sudoku.DALService
{
    public class SudokuOperation : InterfaceToDAL
    {
        MainSudoku MSudoku = MainSudoku.Instance;

        Board InterfaceToDAL.GetBoard(int ProfileId)
        {        
            return MSudoku.GetBoard(MSudoku.checkProfileId(ProfileId));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SudokuOperation() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        Board InterfaceToDAL.SetValue(int i, int j, int value, int ProfileId)
        {
            return MSudoku.SetValueInProfile(i, j, value, MSudoku.checkProfileId(ProfileId));
        }

        bool InterfaceToDAL.Verify(int ProfileId)
        {
            return MSudoku.Verify(MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.ShowSolution(int ProfileId)
        {
            return MSudoku.ShowSolution(MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.Recovery(int ProfileId)
        {
            return MSudoku.Recovery(MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.UndoStep(int ProfileId)
        {
            return MSudoku.UndoStep(MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.NewGame(int ProfileId)
        {
            return MSudoku.NewGame(MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.OpenCell(int i, int j, int ProfileId)
        {
            return MSudoku.OpenCell(i, j, MSudoku.checkProfileId(ProfileId));
        }

        Board InterfaceToDAL.ToBegin(int ProfileId)
        {
            return MSudoku.ToBegin(MSudoku.checkProfileId(ProfileId));
        }
        #endregion
    }
}
