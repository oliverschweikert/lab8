using backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
public class SuperSecureImageController : BaseController
{
  private readonly AuthContext _db;

  public SuperSecureImageController(AuthContext db)
  {
    _db = db;
  }

  [HttpGet]
  [Authorize(Policy = "IsAdmin")]
  public ActionResult<List<SuperSecureImageResponse>> GetImages()
  {
    var dbImages = _db.SuperSecureImages.ToList();
    var responseImages = dbImages.Select(image => new SuperSecureImageResponse(image));
    return new JsonResult(responseImages);
  }

  [HttpGet("{id}")]
  public ActionResult<SuperSecureImageResponse> GetImage(int id)
  {
    var dbImage = _db.SuperSecureImages.Where(image => image.Id == id).SingleOrDefault();
    if (dbImage == null)
      return BadRequest();
    var responseImage = new SuperSecureImageResponse(dbImage);
    return new JsonResult(responseImage);
  }
}