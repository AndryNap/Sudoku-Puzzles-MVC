using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sudoku.IModelsToDAL;
using Sudoku.DALService;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        IModelsToDAL.InterfaceToDAL SudokuService = new Sudoku.DALService.SudokuOperation();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBoard(int prof = 0)
        {
            var model = SudokuService.GetBoard(prof);
            
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
        
        [HttpPost]
        public JsonResult SetBoard(int i = -1, int j = -1, int value = 0, int prof = 0)
        {
            var model = SudokuService.SetValue( i, j, value, prof);
        
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult Verify(int prof = 0)
        {
            bool result = SudokuService.Verify(prof);
            
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public JsonResult ShowSolution(int prof = 0)
        {
            var model = SudokuService.ShowSolution(prof);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult Recovery(int prof = 0)
        {
            var model = SudokuService.Recovery(prof);
            
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult UndoStep(int prof = 0)
        {
            var model = SudokuService.UndoStep(prof);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult NewGame(int prof = 0)
        {
            var model = SudokuService.NewGame(prof);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult OpenCell(int i = -1, int j = -1, int prof = 0)
        {
            var model = SudokuService.OpenCell( i, j, prof);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        [HttpPost]
        public JsonResult ToBegin(int prof = 0)
        {
            var model = SudokuService.ToBegin(prof);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
    }
}