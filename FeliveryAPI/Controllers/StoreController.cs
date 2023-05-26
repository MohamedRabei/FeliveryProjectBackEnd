﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeliveryAPI.Data;
using FeliveryAPI.Models;
using FeliveryAPI.Repository;


namespace FeliveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        public IStoreService ParentStoretRepo { get; set; }
        public StoreController(IStoreService parentStoretRepo, IWebHostEnvironment environment)
        {
            ParentStoretRepo = parentStoretRepo;
            _environment = environment;
        }


        [HttpGet]
        public ActionResult<List<Restaurant>> GetRestaurants()
        {

            return ParentStoretRepo.GetAll();
        }
        [HttpGet("{id}")]
        public ActionResult<Restaurant> GetById(int id)
        {
            return ParentStoretRepo.GetDetails(id);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Restaurant restaurant = ParentStoretRepo.GetDetails(id);

            if (restaurant == null)
            {
                return NotFound();
            }
            ParentStoretRepo.Delete(id);
            return Ok(restaurant);
        }
        [HttpPut]
        public ActionResult Put(Restaurant restaurant)
        {
            if (restaurant != null && restaurant.Id != 0)
            {
                ParentStoretRepo.Update(restaurant);
                return Ok(restaurant);
            }
            return NotFound();
        }

    
        //Upload Images
        [HttpPost("uploadImage")]
       
        public async Task<ActionResult> UploadImage(IFormFileCollection uploadFiles)
        {
            bool Results =false;
            try
            {
                //var uploadFiles = Request.Form.Files;
                foreach (IFormFile file in uploadFiles)
                {
                    string Filename = file.FileName;
                    string Filepath = GetFilePath(Filename);
                    if (!System.IO.Directory.Exists(Filepath))
                    {
                        System.IO.Directory.CreateDirectory(Filepath);
                    }

                    string imagepath = Filepath + "\\image.png";
                    if (System.IO.Directory.Exists(imagepath))
                    {
                        System.IO.Directory.Delete(imagepath);
                    }
                    using FileStream stream = System.IO.File.Create(imagepath);
                    await file.CopyToAsync(stream);
                    Results = true;
                }
            }
            catch (Exception ex) { }
            return Ok(Results);
        }
        [NonAction]
        private string GetFilePath(string ProductCode)
        {
            return this._environment.WebRootPath+ "\\Uploads\\Product\\" + ProductCode;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registeration([FromBody] RegData Data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await ParentStoretRepo.Register(Data);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
            //return Ok();
            //return Ok(new { token = result.Token, expiration = result.ExpiresOn});
        }


        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await ParentStoretRepo.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await ParentStoretRepo.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
    }
}

        /*[HttpPost]
        public ActionResult Post(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ParentStoretRepo.Insert(restaurant);
                    return Created("url", restaurant);
                    // return 201 & Url is the place where you added the object
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message); // Return 400!
                }
            }
            return BadRequest();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await ParentStoretRepo.RegisterAsync(model);


            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
            //return Ok(new { token = result.Token, expiration = result.ExpiresOn});

        }*/