using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.Controllers
{
    public class FormController : Controller
    {
        private readonly FormRepository _repository;

        public FormController(FormRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetForms()
        {
            // DataTables parameters
            int draw = int.Parse(Request.Query["draw"]);
            int start = int.Parse(Request.Query["start"]);   // 0,10,20,...
            int length = int.Parse(Request.Query["length"]); // page size

            int pageNumber = (start / length) + 1;
            int pageSize = length;

            var (forms, totalCount) = await _repository.GetFormsPagedAsync(pageNumber, pageSize);
            var data = forms.Select(f => new
            {
                f.Id,
                f.Title,
                CreatedDate = f.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Actions = ""
            });

            return Json(new
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,  // no filtering, so same as total
                data = data
            });
        }
        public IActionResult Create()
        {
            var model = new FormViewModel();
            model.Fields.Add(new FieldViewModel());
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormViewModel model)
        {
            //var debugModelJson = System.Text.Json.JsonSerializer.Serialize(
            //    model,
            //    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            //return Content(debugModelJson, "application/json");

            //var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            //var json = System.Text.Json.JsonSerializer.Serialize(formData);

            //return Content(json, "application/json");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var form = new Form
            {
                Title = model.Title,
                Fields = model.Fields.Select(f => new FormField
                {
                    Label = f.Label,
                    IsRequired = f.IsRequired,
                    SelectedOption = f.SelectedOption
                }).ToList()
            };
            await _repository.SaveFormAsync(form);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Preview(int id)
        {
            var form = await _repository.GetFormByIdAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }
    }
}
