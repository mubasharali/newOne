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
    
    public partial class QuestionReplyVote
    {
        public int Id { get; set; }
        public int questionReplyId { get; set; }
        public bool isUp { get; set; }
        public string votedBy { get; set; }
    
        public virtual QuestionReply QuestionReply { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}