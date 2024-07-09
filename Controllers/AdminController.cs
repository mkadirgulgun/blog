using BlogApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

namespace BlogApp.Controllers
{
    public class AdminController : Controller
    {
        string connectionString = "TrustServerCertificate=True";

        public IActionResult Index()
        {
            using var connection = new SqlConnection(connectionString);
            var posts = connection.Query<Post>("SELECT id, title, created_date as CreatedDate, updated_date as UpdatedDate FROM posts").ToList();

            return View(posts);
        }
        public IActionResult Yorumlar()
        {
            return View();
        }
        public IActionResult Sil(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM posts WHERE id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Post model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalı işlem yaptın";
                return View("Message");
            }

            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO posts (title, summary, detail, created_date, updated_date, ImgUrl) VALUES (@Title, @Summary, @Detail, @CreatedDate, @UpdatedDate, @ImgUrl)";

            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageName);
            using var stream = new FileStream(path, FileMode.Create);
            model.Image.CopyTo(stream);
            model.ImgUrl = imageName;
            var data = new
            {
                model.Title,
                model.Summary,
                model.Detail,
                model.CreatedDate,
                model.UpdatedDate,
                model.ImgUrl,
            };

            var rowsAffected = connection.Execute(sql, data);


            ViewBag.MessageCssClass = "alert-success";
            ViewBag.Message = "Eklendi.";
            return View("Message");
        }

        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var post = connection.QuerySingleOrDefault<Post>("SELECT * FROM posts WHERE id = @Id", new { Id = id });

            return View(post);
        }

        [HttpPost]
        public IActionResult Duzenle(Post model)
        {
            using var connection = new SqlConnection(connectionString);

            var imageName = model.ImgUrl;
            if (model.Image != null)
            {
                imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageName);
                using var stream = new FileStream(path, FileMode.Create);
                model.Image.CopyTo(stream);
            }

            var sql = "UPDATE posts SET title=@Title, summary=@Summary, detail=@Detail, updated_date=@UpdatedDate, ImgUrl = @ImgUrl WHERE id = @Id";
            
            var parameters = new
            {
                model.Title,
                model.Summary,
                model.Detail,
                UpdatedDate = DateTime.Now,
                model.Id,
                model.ImgUrl,

            };
            var affectedRows = connection.Execute(sql, parameters);

            ViewBag.Message = "Güncellendi.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }

        public IActionResult TumYorumlariGoster()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM comments WHERE IsApproved = 1";
                var comments = connection.Query<PostComment>(sql).ToList();

                return View(comments);
            }
        }
        public IActionResult YorumGoster()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM comments WHERE IsApproved = 0";
                var comments = connection.Query<PostComment>(sql).ToList();

                return View(comments);

            }
        }

        public IActionResult YorumOnayla(int? id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "UPDATE comments SET IsApproved = 1 WHERE Id = @Id";

            var affectedRows = connection.Execute(sql, new { Id = id });

            ViewBag.Message = "Yorum Onaylandı.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }
        public IActionResult YorumSil(int? id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM comments WHERE Id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            ViewBag.Message = "Silindi.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }
    }
}
