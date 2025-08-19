using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class MessageDto
    {
        public int Id { get; set; }


        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PostingDate { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "The {0} field must be at most {1} characters long.")]
        [Display(Name = "Title")]
        public string MessageTitle { get; set; }


        [Required]
        [StringLength(400, ErrorMessage = "The {0} field must be at most {1} characters long.")]
        public string MessageContent { get; set; }

        [Required]
        public string SenderEmail { get; set; }

        [Required]
        public string ReceiverEmail { get; set; }


        public EnumDto Status { get; set; }
    }
}
