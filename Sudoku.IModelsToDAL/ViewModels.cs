using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.IModelsToDAL
{
    public class Board
    {
        public int Variant { get; set; }

        public string Message { get; set; }
        public Cells[,] SBoard { get; set; }

        public int StepCount { get; set; }

        public void SetVariant(int variant)
        {
            Variant = variant;
        }

        public void SetMessage( string message )
        {
            Message = message;
        }

        public void Step()
        {
            StepCount++;
        }
    }

    public class Cells
    {
        public int i { get; set; }
        public int j { get; set; }
        public int value { get; set; }
        public bool given { get; set; }

        public IEnumerable<int> allow { get; set; }
    }
}
