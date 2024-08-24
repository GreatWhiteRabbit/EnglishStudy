using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnglishStudy.DTO
{

    /// <summary>
    /// 封装word集合
    /// </summary>
    public class WordDTO
    {
        public WordDTO()
        {
        }

        public WordDTO(string words, string paraphrase, string phonetic)
        {
            Words = words;
            Paraphrase = paraphrase;
            Phonetic = phonetic;
        }

        public string Words { get; set; }


        public string Phonetic { get; set; }


        public string Paraphrase { get; set; }


    }
}
