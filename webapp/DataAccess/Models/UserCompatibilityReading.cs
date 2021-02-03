﻿using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.CompatibilityReading, PluralName = Globalisation.Strings.Names.CompatibilityReadings)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserRelationshipCompatibilityReading : ObjectBase, IUserData
    {
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("UserMembership")]
        public int UserMembershipId { get; set; }

        [ForeignKey("UserCreditPack")]
        public int? UserCreditPackId { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        public string SecondName { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DateOfBirthLabel)]
        public DateTime FirstDateOfBirth { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public EGender FirstGender { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DateOfBirthLabel)]
        public DateTime SecondDateOfBirth { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public EGender SecondGender { get; set; }

        public bool IsHideSexuality { get; set; }

        public virtual User User { get; set; }

        public virtual UserMembership UserMembership { get; set; }

        public virtual UserCreditPack UserCreditPack { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }
    }
}
