using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blog_application.Models;
using Microsoft.Data.Sqlite;
using blog_application.Models.ViewModels;

namespace blog_application.Controllers;

public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    private readonly IConfiguration _configuration;

    public PostsController(ILogger<PostsController> logger , IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var postListViewModel = GetAllPosts();
        return View(postListViewModel);
    }

    public IActionResult NewPost()
    {
        return View();  
    }

    public IActionResult ViewPost(int id)
    {
        var post = GetPostById(id);
        var postViewModel = new PostViewModel();
        postViewModel.Post = post;
        return View(postViewModel);
    }

    public IActionResult EditPost(int id ) 
    {
        var post = GetPostById(id);
        var postViewModel = new PostViewModel();
        postViewModel.Post = post;
        return  View(postViewModel);
    }

    

    internal PostViewModel GetAllPosts()
    {
        List<PostModel> postList = new();

        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("BlogDataContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM post";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            postList.Add(new PostModel
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Body = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                UpdatedAt = reader.GetDateTime(4)
                            }
                            );
                        }
                    } else {
                        return new PostViewModel {PostList= postList};
                    }
                }
            }

        }

            return new PostViewModel
            {
            PostList =  postList 
            };

    }



internal PostModel GetPostById(int id)
    {
        PostModel post = new();

        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("BlogDataContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM post WHERE Id = '{id}'";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            
                                post.Id = reader.GetInt32(0);
                                post.Title = reader.GetString(1);
                                post.Body = reader.GetString(2);
                                post.CreatedAt = reader.GetDateTime(3);
                                post.UpdatedAt = reader.GetDateTime(4);
                        }
                    } else return post;
                    
                }
            }

        }

            return post;

    }

    public IActionResult Insert(PostModel post)
    {
        post.CreatedAt = DateTime.Now;  
        post.UpdatedAt = DateTime.Now;

        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("BlogDataContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"INSERT INTO post (title , body , createdAt , updatedAt)  VALUES ('{post.Title}' , '{post.Body}' , '{post.CreatedAt}' , '{post.UpdatedAt}' )";

                try {
                    command.ExecuteNonQuery();
                } catch(Exception ex){
                    Console.WriteLine(ex.Message);
                }

        
            }
        }

        return RedirectToAction(nameof(Index));
    }

    
    public ActionResult Update(PostModel post)
    {
        post.UpdatedAt = DateTime.Now;

        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("BlogDataContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"UPDATE post SET Title = '{post.Title}', Body = '{post.Body}', UpdatedAt = '{post.UpdatedAt}' WHERE Id = '{post.Id}'";
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection connection =
                new SqliteConnection(_configuration.GetConnectionString("BlogDataContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"DELETE from post WHERE Id = '{id}'";
                command.ExecuteNonQuery();
            }
        }

        return Json(new Object{});
    }
}
