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
    
    public partial class CarModel
    {
        public CarModel()
        {
            this.CarAds = new HashSet<CarAd>();
        }
    
        public int Id { get; set; }
        public int brandId { get; set; }
        public string model { get; set; }
        public System.DateTime time { get; set; }
        public string addedBy { get; set; }
        public string status { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual CarBrand CarBrand { get; set; }
        public virtual ICollection<CarAd> CarAds { get; set; }
    }
}