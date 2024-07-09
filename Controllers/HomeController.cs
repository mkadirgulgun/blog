using BlogApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = "TrustServerCertificate=True";
        public IActionResult Index()
        {
            // Connect to the database 
            using var connection = new SqlConnection(connectionString);
            var posts = connection.Query<Post>("SELECT * FROM posts ORDER BY updated_date DESC").ToList();

            return View(posts);
        }

        public IActionResult Detay(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var postModel = new PostModel();

            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM posts WHERE id = @id";
                var post = connection.QuerySingleOrDefault<Post>(sql, new { id = id });

                postModel.Post = post;

            }
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM comments WHERE postId = @id AND IsApproved = 1 ";
                var comments = connection.Query<PostComment>(sql, new { id = id }).ToList();

                postModel.Comments = comments;

            }

            return View(postModel);
        }
        [HttpPost]
        public IActionResult YorumEkle(PostComment model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            model.CreatedDate = DateTime.Now;

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO comments (name, comment, postId, createddate) VALUES (@Name, @Comment, @PostId, @CreatedDate)";
            try
            {
                var affectedRows = connection.Execute(sql, model);
                return RedirectToAction("Detay", new { id = model.PostId });
            }
            catch
            {
                return RedirectToAction("Index");

            }
        }
        public IActionResult Hakkimda()
        {
            return View();
        }

        public IActionResult Foto()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Foto(FotoModel model)
        {
           
            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageName);
            using var stream = new FileStream(path, FileMode.Create);
            model.Image.CopyTo(stream);
            ViewBag.Image = $"/uploads/{imageName}";
            return View();

            //if (model.Image != null && model.Image.Length > 0)
            //{
            //    // Dosya boþ deðil, iþleme devam edilebilir.
            //    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageName);
            //    using var stream = new FileStream(path, FileMode.Create);
            //    model.Image.CopyTo(stream);
            //    ViewBag.Image = $"/uploads/{imageName}";
            //}
            //else
            //{
            //    // Dosya boþ veya yüklenmemiþ, hata mesajý gösterilebilir.
            //    ModelState.AddModelError("Image", "Lütfen geçerli bir dosya yükleyin.");
            //}

            //return View();
        }
    }
}
