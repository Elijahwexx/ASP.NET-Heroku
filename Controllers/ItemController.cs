using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/items")]
    public class ItemController : Controller
    {
        private IItemRepository m_repo;

        public ItemController(IItemRepository repository)
        {
            m_repo = repository;
        }

        /// <summary>
        /// Get all items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Json(m_repo.GetItems());
        }

        /// <summary>
        /// Get one item using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = m_repo.GetItem(id);

            if (item == null)
                return NotFound(id);

            return Json(item);
        }

        /// <summary>
        /// create new item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(item);
            }

            var result = m_repo.Create(item);
            return Json(result);
        }

        /// <summary>
        /// Update existiong item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put([FromBody]Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(item);
            }

            var result = m_repo.Update(item);
            return Json(result);
        }

        /// <summary>
        /// delet item using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deletedItem = m_repo.Delete(id);
            if (deletedItem == null)
                return NotFound();

            return Json(deletedItem);
        }
    }
}