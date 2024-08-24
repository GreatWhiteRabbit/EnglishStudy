using EnglishStudy.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EnglishStudy.DTO {
    public class PassageDTO {

        public int PassageId { get; set; }
  
        public string Title { get; set; }
  
        public string Content { get; set; }

        public List<QuestionDTO> QuestionList { get; set; } = new List<QuestionDTO>();
    }
}
