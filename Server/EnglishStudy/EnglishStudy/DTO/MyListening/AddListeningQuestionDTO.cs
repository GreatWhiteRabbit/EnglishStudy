using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EnglishStudy.DTO.MyListening {
    // 用于添加listeningQuestion的封装类
    public class AddListeningQuestionDTO {

        
        public string OptionsA { get; set; }


        public string OptionsB { get; set; }
        

        public string OptionsC { get; set; }


        public string OptionsD { get; set; }
        
        public string Title { get; set; }

        public string Answer { get; set; }


    }
}
