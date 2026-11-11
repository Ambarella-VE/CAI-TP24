// [TP Microservicios E-Commerce v7] Implementa reglas de negocio de Users.API solicitadas por la consigna.
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Shared.Contracts.Errors;
using Users.API.DTOs;
using Users.API.Models;

namespace Users.API.Services;

public class UserService
{
    private readonly List<User> _users = [];
    private readonly PasswordHasher<User> _hasher = new();

    public UserResponse Register(RegisterUserRequest request)
    {
        // La validación temprana devuelve el código de negocio en vez de filtrar un error técnico de persistencia.
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Apellido) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) ||
            !new EmailAddressAttribute().IsValid(request.Email.Trim()))
            throw new DomainException("USR-002");

        var email = request.Email.Trim().ToLowerInvariant();
        if (_users.Any(user => user.Email == email)) throw new DomainException("USR-001");

        var user = new User { Id = Guid.NewGuid(), FirstName = request.Nombre.Trim(), LastName = request.Apellido.Trim(), Email = email, RegisteredAt = DateTime.UtcNow };
        // Nunca se guarda ni se devuelve la contraseña original: sólo un hash verificable.
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        _users.Add(user);
        return ToResponse(user);
    }

    public UserResponse Login(LoginRequest request)
    {
        var user = _users.FirstOrDefault(item => item.Email == request.Email.Trim().ToLowerInvariant()) ?? throw new DomainException("USR-003");
        if (!user.Active) throw new DomainException(user.BlockReason == BlockReason.Fraud ? "USR-005" : "USR-004");
        if (_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            // Los intentos son consecutivos; un login correcto los reinicia más abajo.
            user.FailedAttempts++;
            if (user.FailedAttempts >= 3) { user.Active = false; user.BlockReason = BlockReason.FailedAttempts; }
            throw new DomainException(user.Active ? "USR-003" : "USR-004");
        }
        // Un acceso válido demuestra que no corresponde mantener un bloqueo por intentos anteriores.
        user.FailedAttempts = 0;
        return ToResponse(user);
    }

    public UserResponse GetById(Guid id)
    {
        var user = _users.FirstOrDefault(item => item.Id == id) ?? throw new DomainException("USR-003");
        return ToResponse(user);
    }

    private static UserResponse ToResponse(User user) => new(user.Id, user.FirstName, user.LastName, user.Email, user.RegisteredAt, user.Active);
}
