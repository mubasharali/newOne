//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inspinia_MVC5_SeedProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            this.Ads = new HashSet<Ad>();
            this.Answers = new HashSet<Answer>();
            this.AnswerReplies = new HashSet<AnswerReply>();
            this.AnswerReplyVotes = new HashSet<AnswerReplyVote>();
            this.AnswerVotes = new HashSet<AnswerVote>();
            this.AspNetUserClaims = new HashSet<AspNetUserClaim>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogin>();
            this.Bids = new HashSet<Bid>();
            this.Chats = new HashSet<Chat>();
            this.Chats1 = new HashSet<Chat>();
            this.Cities = new HashSet<City>();
            this.Cities1 = new HashSet<City>();
            this.Comments = new HashSet<Comment>();
            this.CommentReplies = new HashSet<CommentReply>();
            this.CommentReplyVotes = new HashSet<CommentReplyVote>();
            this.CommentVotes = new HashSet<CommentVote>();
            this.CompanyAnswers = new HashSet<CompanyAnswer>();
            this.CompanyAnswerReplies = new HashSet<CompanyAnswerReply>();
            this.CompanyQuestions = new HashSet<CompanyQuestion>();
            this.ReviewReplies = new HashSet<ReviewReply>();
            this.FollowCompanies = new HashSet<FollowCompany>();
            this.FollowQuestions = new HashSet<FollowQuestion>();
            this.FollowTags = new HashSet<FollowTag>();
            this.Friends = new HashSet<Friend>();
            this.Friends1 = new HashSet<Friend>();
            this.LaptopBrands = new HashSet<LaptopBrand>();
            this.LaptopModels = new HashSet<LaptopModel>();
            this.Mobiles = new HashSet<Mobile>();
            this.MobileModels = new HashSet<MobileModel>();
            this.popularPlaces = new HashSet<popularPlace>();
            this.popularPlaces1 = new HashSet<popularPlace>();
            this.Questions = new HashSet<Question>();
            this.QuestionReplies = new HashSet<QuestionReply>();
            this.QuestionReplyVotes = new HashSet<QuestionReplyVote>();
            this.QuestionVotes = new HashSet<QuestionVote>();
            this.Reporteds = new HashSet<Reported>();
            this.ReportedQuestions = new HashSet<ReportedQuestion>();
            this.ReportedTags = new HashSet<ReportedTag>();
            this.Reviews = new HashSet<Review>();
            this.SaveAds = new HashSet<SaveAd>();
            this.Tags = new HashSet<Tag>();
            this.Tags1 = new HashSet<Tag>();
            this.AspNetRoles = new HashSet<AspNetRole>();
            this.CompanyAds = new HashSet<CompanyAd>();
            this.Companies = new HashSet<Company>();
            this.ReviewVotes = new HashSet<ReviewVote>();
            this.CarBrands = new HashSet<CarBrand>();
            this.CarModels = new HashSet<CarModel>();
            this.BikeBrands = new HashSet<BikeBrand>();
            this.BikeModels = new HashSet<BikeModel>();
            this.Feedbacks = new HashSet<Feedback>();
        }
    
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> since { get; set; }
        public Nullable<int> reputation { get; set; }
        public Nullable<bool> hideEmail { get; set; }
        public Nullable<bool> hidePhoneNumber { get; set; }
        public string dpExtension { get; set; }
        public string gender { get; set; }
        public Nullable<System.DateTime> dateOfBirth { get; set; }
        public string about { get; set; }
        public string city { get; set; }
        public Nullable<bool> hideDateOfBirth { get; set; }
        public Nullable<bool> hideFriends { get; set; }
        public Nullable<bool> IsPasswordSaved { get; set; }
        public string status { get; set; }
    
        public virtual ICollection<Ad> Ads { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<AnswerReply> AnswerReplies { get; set; }
        public virtual ICollection<AnswerReplyVote> AnswerReplyVotes { get; set; }
        public virtual ICollection<AnswerVote> AnswerVotes { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Chat> Chats1 { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<City> Cities1 { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CommentReply> CommentReplies { get; set; }
        public virtual ICollection<CommentReplyVote> CommentReplyVotes { get; set; }
        public virtual ICollection<CommentVote> CommentVotes { get; set; }
        public virtual ICollection<CompanyAnswer> CompanyAnswers { get; set; }
        public virtual ICollection<CompanyAnswerReply> CompanyAnswerReplies { get; set; }
        public virtual ICollection<CompanyQuestion> CompanyQuestions { get; set; }
        public virtual ICollection<ReviewReply> ReviewReplies { get; set; }
        public virtual ICollection<FollowCompany> FollowCompanies { get; set; }
        public virtual ICollection<FollowQuestion> FollowQuestions { get; set; }
        public virtual ICollection<FollowTag> FollowTags { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Friend> Friends1 { get; set; }
        public virtual ICollection<LaptopBrand> LaptopBrands { get; set; }
        public virtual ICollection<LaptopModel> LaptopModels { get; set; }
        public virtual ICollection<Mobile> Mobiles { get; set; }
        public virtual ICollection<MobileModel> MobileModels { get; set; }
        public virtual ICollection<popularPlace> popularPlaces { get; set; }
        public virtual ICollection<popularPlace> popularPlaces1 { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuestionReply> QuestionReplies { get; set; }
        public virtual ICollection<QuestionReplyVote> QuestionReplyVotes { get; set; }
        public virtual ICollection<QuestionVote> QuestionVotes { get; set; }
        public virtual ICollection<Reported> Reporteds { get; set; }
        public virtual ICollection<ReportedQuestion> ReportedQuestions { get; set; }
        public virtual ICollection<ReportedTag> ReportedTags { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<SaveAd> SaveAds { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Tag> Tags1 { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
        public virtual ICollection<CompanyAd> CompanyAds { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<ReviewVote> ReviewVotes { get; set; }
        public virtual ICollection<CarBrand> CarBrands { get; set; }
        public virtual ICollection<CarModel> CarModels { get; set; }
        public virtual ICollection<BikeBrand> BikeBrands { get; set; }
        public virtual ICollection<BikeModel> BikeModels { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}