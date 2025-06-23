using BCrypt.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;
using Ustagram.Domain.Model;
using Ustagram.Infrastructure.Persistance;

namespace Ustagram.Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IElasticSerachService _elasticSerachService;

    public UserService(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IElasticSerachService elasticSerachService)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
        _elasticSerachService = elasticSerachService;
    }

    public async Task<string> CreateUser(UserDTO userDto)
    {
        if (userDto.Password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.");



        var pictureFile = userDto.Photo;
        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos");
        string fileName = "";

        try
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                Debug.WriteLine("Directory created successfully.");
            }

            fileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
            filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pictureFile.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            return "Error in saving image!";
        }
        var newUser = new User
        {
            FullName = userDto.FullName,
            Username = userDto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password, workFactor: 11),
            Phone = userDto.Phone,
            Location = userDto.Location ?? "",
            PhotoPath = "/UserPhotos/" + fileName,
            Dob = userDto.Dob ?? "",
            Status = userDto.Status ?? "",
            MasterType = userDto.MasterType ?? "",
            Bio = userDto.Bio ?? "",
            Experience = userDto.Experience,
            TelegramUrl = userDto.TelegramUrl ?? "",
            InstagramUrl = userDto.InstagramUrl ?? ""
        };

        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();

        await _elasticSerachService.AddElasticUserAsync(newUser);

        return "User Created!";
    }

    public async Task<UserResponseDTO> GetUserById(Guid userId)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return MapToUserResponseDTO(user);
    }

    public async Task<string> UpdateUser(Guid userId, UserDTO userDto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found");


        if (userDto.Photo != null)
        {

            var pictureFile = userDto.Photo;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos");
            string fileName = "";

            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    Debug.WriteLine("Directory created successfully.");
                }

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return "Error in saving image!";
            }

            user.PhotoPath = "/UserPhotos/" + fileName;

        }

        user.FullName = userDto.FullName;
        user.Username = userDto.Username;
        user.Phone = userDto.Phone;
        user.Location = userDto.Location;
        user.Dob = userDto.Dob;
        user.Status = userDto.Status;
        user.MasterType = userDto.MasterType;
        user.Bio = userDto.Bio;
        user.Experience = userDto.Experience;
        user.TelegramUrl = userDto.TelegramUrl;
        user.InstagramUrl = userDto.InstagramUrl;

        if (!string.IsNullOrEmpty(userDto.Password) && userDto.Password != "UNCHANGED")
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }

        await _db.SaveChangesAsync();

        return "User updated successfully";
    }

    public async Task<string> DeleteUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return "User not found";

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        
        await _elasticSerachService.DeleteElasticUserAsync(user);
        return "User Deleted!";
    }

    public async Task<List<UserResponseDTO>> GetAllUsers()
    {
        var users = await _db.Users.ToListAsync();
        return users.Select(u => MapToUserResponseDTO(u)).ToList();
    }

    public async Task<UserResponseDTO> GetUserByUsername(string username)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return null;

        return MapToUserResponseDTO(user);
    }

    public async Task<User> GetUserEntityByUsername(string username)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<UserResponseDTO>> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<UserResponseDTO>();

        var users = await _db.Users
            .Where(u => u.Username.Contains(term) || u.FullName.Contains(term))
            .Take(10)
            .ToListAsync();

        return users.Select(u => MapToUserResponseDTO(u)).ToList();
    }

    private UserResponseDTO MapToUserResponseDTO(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Phone = user.Phone,
            Location = user.Location,
            PhotoPath = user.PhotoPath,
            Dob = user.Dob,
            Status = user.Status,
            MasterType = user.MasterType,
            Bio = user.Bio,
            Experience = user.Experience,
            TelegramUrl = user.TelegramUrl,
            InstagramUrl = user.InstagramUrl
        };
    }
}