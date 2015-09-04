﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOTD.Utility.ExceptionHelpers;

namespace CritterHeroes.Web.Data.Models
{
    public class PersonPhone
    {
        protected PersonPhone()
        {
        }

        internal PersonPhone(Person person, PhoneType phoneType, string phoneNumber)
            : this(person, phoneType, phoneNumber, null)
        {
        }

        internal PersonPhone(Person person, PhoneType phoneType, string phoneNumber, string phoneExtension)
        {
            ThrowIf.Argument.IsNull(person, nameof(person));
            ThrowIf.Argument.IsNull(phoneType, nameof(phoneType));
            ThrowIf.Argument.IsNullOrEmpty(phoneNumber, nameof(phoneNumber));

            this.PhoneType = phoneType;
            this.PhoneTypeID = phoneType.ID;

            this.Person = person;
            this.PersonID = person.ID;

            this.PhoneNumber = phoneNumber;
            this.PhoneExtension = phoneExtension;
        }

        public int ID
        {
            get;
            private set;
        }

        public string PhoneNumber
        {
            get;
            set;
        }

        public string PhoneExtension
        {
            get;
            set;
        }

        public int PhoneTypeID
        {
            get;
            private set;
        }

        public virtual PhoneType PhoneType
        {
            get;
            private set;
        }

        public int PersonID
        {
            get;
            private set;
        }

        public virtual Person Person
        {
            get;
            private set;
        }

        public void ChangePhoneType(PhoneType phoneType)
        {
            ThrowIf.Argument.IsNull(phoneType, nameof(phoneType));

            this.PhoneType = phoneType;
            this.PhoneTypeID = phoneType.ID;
        }
    }
}
