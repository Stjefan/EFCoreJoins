using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EFCoreJoins
{
    class Program
    {
        static void Main(string[] args)
        {
            bool setupDb = false;
            if (setupDb)
            {
                System.IO.File.Delete("MyDb.sqlite");
            }
            
            
            using (var context = new EFCoreDemoContext())
            {
                
                if (setupDb)
                {
                    context.Database.EnsureCreated();
                    var relation = new AuthorBiography()
                    {
                        DateOfBirth = DateTime.Now
                    };
                    var author = new Author
                    {
                        FirstName = "William",
                        LastName = "Shakespeare",
                        Books = new List<Book>
                    {
                        new Book { Title = "Hamlet", GlobalId = 15, BookId = 1},
                        new Book { Title = "Othello", GlobalId = 15, BookId = 2},
                        new Book { Title = "MacBeth", GlobalId = 15, BookId = 3 }
                    }
                    };
                    author.Biography = relation;
                    context.Add(author);
                    context.SaveChanges();
                } else
                {
                    // var myAuthors = context.Authors.Include(a => a.Biography).Include(a => a.Books).FirstAsync();
                    var myBooks = context.Books.Include(b => b.Author).FirstAsync().Result;
                }
                
            }
        }
    }

    public class Book
    {
        public int BookId { get; set; }
        public int GlobalId { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Book> Books { get; set; }
    }
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public AuthorBiography Biography { get; set; }
    }

    /*
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AuthorBiography Biography { get; set; }
    }
    */
    public class AuthorBiography
    {
        public int AuthorBiographyId { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Nationality { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int AuthorRef { get; set; }
    }

    public class EFCoreDemoContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=MyDb.sqlite");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => new { b.BookId, b.GlobalId });

            modelBuilder.Entity<Author>()
                .HasOne(a => a.Biography)
                .WithOne(b => b.Author)
                .HasForeignKey<AuthorBiography>(b => b.AuthorRef);
        }
    }
}
