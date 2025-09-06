//using ClassLibrary.DtoModels;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ProjectCondoManagement.Data.Entites.CondosDb;
//using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
//using ProjectCondoManagement.Helpers;

//namespace ProjectCondoManagement.Controllers
//{
//    public class MeetingController : Controller
//    {
//        private readonly IMeetingRepository _meetingRepository;
//        private readonly DataContextCondos _dataContextCondos;
//        private readonly IConverterHelper _converterHelper;

//        public MeetingController(IMeetingRepository meetingRepository, DataContextCondos dataContextCondos, IConverterHelper converterHelper)
//        {
//            _meetingRepository = meetingRepository;
//            _dataContextCondos = dataContextCondos;
//            _converterHelper = converterHelper;
//        }


//        [HttpGet ("GetCondoMeetings/{id}")]
//        public async Task<ActionResult<List<MeetingDto>> GetCondoMeetings(int id)
//        {
//           var meetings = _meetingRepository.GetAll(_dataContextCondos).Where(m => m.CondominiumId == id).ToList();

//           return meetings.Select(m => _converterHelper.ToMeetingDto(m)).ToList();  
//        }

//        // GET: MeetingController/Details/5
//        public ActionResult Details(int id)
//        {
//            return View();
//        }

//        // GET: MeetingController/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: MeetingController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: MeetingController/Edit/5
//        public ActionResult Edit(int id)
//        {
//            return View();
//        }

//        // POST: MeetingController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(int id, IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: MeetingController/Delete/5
//        public ActionResult Delete(int id)
//        {
//            return View();
//        }

//        // POST: MeetingController/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(int id, IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//    }
//}
