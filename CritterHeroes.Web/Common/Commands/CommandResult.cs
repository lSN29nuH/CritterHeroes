﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using TOTD.Utility.ExceptionHelpers;

namespace CritterHeroes.Web.Common.Commands
{
    public class CommandResult
    {
        protected CommandResult()
        {
            this.Errors = new Dictionary<string, List<string>>();
        }

        public static CommandResult Success()
        {
            CommandResult result = new CommandResult();
            result.Succeeded = true;
            return result;
        }

        public static CommandResult Failed()
        {
            CommandResult result = new CommandResult();
            result.Succeeded = false;
            return result;
        }

        public static CommandResult Failed(string errorKey, string errorMessage)
        {
            return Failed().AddError(errorKey, errorMessage);
        }

        public static CommandResult Failed(string errorKey, IEnumerable<string> errorMessages)
        {
            return Failed().AddErrors(errorKey, errorMessages);
        }

        public static CommandResult FromIdentityResult(IdentityResult identityResult, string errorKey)
        {
            if (identityResult.Succeeded)
            {
                return Success();
            }
            else
            {
                return Failed().AddIdentityResultErrors(errorKey, identityResult);
            }
        }

        public bool Succeeded
        {
            get;
            private set;
        }

        public IDictionary<string, List<string>> Errors
        {
            get;
            private set;
        }

        public CommandResult AddError(string key, string error)
        {
            ThrowIf.Argument.IsNull(key, "key");
            ThrowIf.Argument.IsNullOrEmpty(error, "error");

            List<string> errors;
            if (!Errors.TryGetValue(key, out errors))
            {
                errors = new List<string>();
                Errors[key] = errors;
            }

            errors.Add(error);

            return this;
        }

        public CommandResult AddIdentityResultErrors(string key, IdentityResult identityResult)
        {
            ThrowIf.Argument.IsNull(identityResult, "identityResult");
            return AddErrors(key, identityResult.Errors);
        }

        public CommandResult AddErrors(string key, IEnumerable<string> errors)
        {
            ThrowIf.Argument.IsNull(key, "key");
            ThrowIf.Argument.IsNull(errors, "errors");

            foreach (string error in errors)
            {
                AddError(key, error);
            }

            return this;
        }
    }
}