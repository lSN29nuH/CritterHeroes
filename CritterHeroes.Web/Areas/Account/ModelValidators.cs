﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Common.Validation;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Storage;
using FluentValidation;
using FluentValidation.Results;
using TOTD.Utility.StringHelpers;

namespace CritterHeroes.Web.Areas.Account
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter your email address.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter a password.");
        }
    }

    public class EditProfileModelValidator : AbstractValidator<EditProfileModel>
    {
        public EditProfileModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please enter your first name.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please enter your last name.");
        }
    }

    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator()
        {
            RuleFor(x => x.ResetPasswordEmail).NotEmpty().WithMessage("Please enter your email address.");
        }
    }

    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter your email address.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter a password.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
        }
    }

    public class ConfirmEmailModelValidator : AbstractValidator<ConfirmEmailModel>
    {
        public ConfirmEmailModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter your email address.");
            RuleFor(x => x.ConfirmationCode).NotEmpty().WithMessage("Please enter the confirmation code from your email.");
        }
    }

    public class EditProfileLoginModelValidator : AbstractValidator<EditProfileLoginModel>
    {
        public EditProfileLoginModelValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter a password.");
        }
    }

    public class EditProfileSecureModelValidator : AbstractValidator<EditProfileSecureModel>
    {
        private IApplicationUserManager _userManager;
        private IHttpUser _httpUser;

        public EditProfileSecureModelValidator(IApplicationUserManager storageContext, IHttpUser httpUser)
        {
            this._userManager = storageContext;
            this._httpUser = httpUser;

            RuleFor(x => x.NewEmail)
              .Cascade(CascadeMode.StopOnFirstFailure)
              .NotEmpty().WithMessage("Please enter your email address.")
              .EmailAddress().WithMessage("Please enter a valid email address.")
              .MustHaveUniqueEmail(_userManager, _httpUser).WithMessage("The email address you entered is not available. Please enter a different email address.");
        }
    }
}