using EnglishStudy.Entity;
using Microsoft.EntityFrameworkCore;

namespace EnglishStudy {
    public class MyDbContext :DbContext {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {

        }

        public DbSet<Word> Words { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Passage> Passages { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<EveryDayEnglish> EveryDayEnglishes { get; set; }

        public DbSet<EveryDayEnglishRecord> EveryDayEnglishRecords { get; set; }

        public DbSet<PassageRecord> PassageRecords { get; set; }

        public DbSet<ListeningPaper> ListeningPapers { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<ListeningQuestion> ListeningQuestions { get; set; }

        public DbSet<ListeningPaperRecord> ListeningPaperRecords { get; set; }

        public DbSet<WordPosition> WordPositions { get; set; }

        public DbSet<WordRecord> WordRecords { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Receive> Receives { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentRecive> CommentRecives { get; set; }

        public DbSet<Resource> Resources { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
           
        }
    }
}
