﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using LISy.Entities;
using LISy.Entities.Documents;
using LISy.Entities.Users;
using LISy.Entities.Users.Patrons;
using LISy.Managers;
using LISy.Managers.DataManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LISyTest.Integrated
{
    [TestClass]
    public class Delivery2
    {
        [TestMethod]
        public void TestCase1()
        {
            DatabaseDataManager.ClearAll();
            LibrarianDataManager.AddUser(new Librarian("LibrarianName", "LibrarianSurname", "80000000000", "Address"),
                "librarian_1", "12345");
            LibrarianDataManager.AddUser(new Faculty("Sergey", "Afonso", "30001", "ViaMargutta, 3"),
                "patron_1", "12345");
            LibrarianDataManager.AddUser(new Student("Nadia", "Teixeira", "30002", "Via Sacra, 13"),
                "patron_2", "12345");
            LibrarianDataManager.AddUser(new Student("Elvira", "Espindola", "30003", "Via del Corso, 22"),
                "patron_3", "12345");

            LibrarianDataManager.AddDocument(new Book("Authors", "Book_1", "Publisher", "Edition", 2018, false, "Keys", "", 100));
            LibrarianDataManager.AddCopy(3, new Copy(1, 1, 1));
            LibrarianDataManager.AddDocument(new Book("Authors", "Book_2", "Publisher", "Edition", 2018, false, "Keys", "", 100));
            LibrarianDataManager.AddCopy(2, new Copy(2, 1, 2));
            LibrarianDataManager.AddDocument(new Book("Authors", "Book_3", "Publisher", "Edition", 2018, false, "Keys", "", 100));
            LibrarianDataManager.AddCopy(1, new Copy(3, 1, 3));
            LibrarianDataManager.AddDocument(new AVMaterial("Authors", "AVMaterial_1", "Keys", "", 100));
            LibrarianDataManager.AddCopy(1, new Copy(4, 1, 4));
            LibrarianDataManager.AddDocument(new AVMaterial("Authors", "AVMaterial_2", "Keys", "", 100));
            LibrarianDataManager.AddCopy(1, new Copy(5, 1, 5));            

            Assert.AreEqual(LibrarianDataManager.GetNumberOfCopies(), 8);
            Assert.AreEqual(LibrarianDataManager.GetNumberOfUsers(), 4);
        }

        [TestMethod]
        public void TestCase2()
        {
            TestCase1();
            LibrarianDataManager.DeleteCopyByDocId(new Copy(1, 1, 1));
            LibrarianDataManager.DeleteCopyByDocId(new Copy(1, 1, 1));
            LibrarianDataManager.DeleteCopyByDocId(new Copy(3, 1, 3));
            Student student = new Student("StudentName", "StudentSurname", "80000000000", "Address")
                { CardNumber = 3};
            LibrarianDataManager.DeleteUser(student);

            int n = LibrarianDataManager.GetNumberOfCopies();
            int s = LibrarianDataManager.GetNumberOfUsers();
            Assert.AreEqual(LibrarianDataManager.GetNumberOfCopies(), 5);
            Assert.AreEqual(LibrarianDataManager.GetNumberOfUsers(), 3);
        }

        [TestMethod]
        public void TestCase3()
        {
            TestCase1();
            IUser faculty = LibrarianDataManager.GetUserById(2);
            Assert.AreEqual(faculty.FirstName, "Sergey");
            Assert.AreEqual(faculty.SecondName, "Afonso");
            Assert.AreEqual(faculty.Phone, "30001");
            Assert.AreEqual(faculty.Address, "ViaMargutta, 3");
            faculty = LibrarianDataManager.GetUserById(4);
            Assert.AreEqual(faculty.FirstName, "Elvira");
            Assert.AreEqual(faculty.SecondName, "Espindola");
            Assert.AreEqual(faculty.Phone, "30003");
            Assert.AreEqual(faculty.Type, "Student");
            Assert.AreEqual(faculty.Address, "Via del Corso, 22");
        }

        [TestMethod]
        public void TestCase4()
        {
            TestCase2();
            IUser faculty = LibrarianDataManager.GetUserById(3);
            Assert.IsNull(faculty);
            faculty = LibrarianDataManager.GetUserById(4);
            Assert.AreEqual(faculty.FirstName, "Elvira");
            Assert.AreEqual(faculty.SecondName, "Espindola");
            Assert.AreEqual(faculty.Phone, "30003");
            Assert.AreEqual(faculty.Type, "Student");
            Assert.AreEqual(faculty.Address, "Via del Corso, 22");            
        }

        [TestMethod]
        public void TestCase5()
        {
            TestCase2();
            PatronDataManager.CheckOutDocument(1, 3);
            foreach (var copy in LibrarianDataManager.GetAllCopiesList())
            {
                if (copy.DocumentID == 1 && copy.PatronID == 3)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void TestCase6()
        {
            TestCase2();            

            PatronDataManager.CheckOutDocument(1, 2);
            PatronDataManager.CheckOutDocument(2, 4);
            PatronDataManager.CheckOutDocument(2, 2);
            
            IUser faculty = LibrarianDataManager.GetUserById(2);
            Assert.AreEqual(faculty.FirstName, "Sergey");
            Assert.AreEqual(faculty.SecondName, "Afonso");
            Assert.AreEqual(faculty.Phone, "30001");
            Assert.AreEqual(faculty.Type, "Faculty");
            Assert.AreEqual(faculty.Address, "ViaMargutta, 3");
            Copy copy = LibrarianDataManager.GetCheckedByUserCopiesList(faculty.CardNumber)[0];
            Assert.AreEqual(copy.DocumentID, 1);

            faculty = LibrarianDataManager.GetUserById(4);
            Assert.AreEqual(faculty.FirstName, "Elvira");
            Assert.AreEqual(faculty.SecondName, "Espindola");
            Assert.AreEqual(faculty.Phone, "30003");
            Assert.AreEqual(faculty.Type, "Student");
            Assert.AreEqual(faculty.Address, "Via del Corso, 22");
            Copy[] copies = LibrarianDataManager.GetAllCopiesList();
            copy = LibrarianDataManager.GetCheckedByUserCopiesList(faculty.CardNumber)[0];
            Assert.AreEqual(copy.DocumentID, 2);
        }

        [TestMethod]
        public void TestCase7()
        {
            TestCase1();

            PatronDataManager.CheckOutDocument(1, 2);
            PatronDataManager.CheckOutDocument(2, 2);
            PatronDataManager.CheckOutDocument(3, 2);
            PatronDataManager.CheckOutDocument(4, 2);

            PatronDataManager.CheckOutDocument(1, 3);
            PatronDataManager.CheckOutDocument(2, 3);
            PatronDataManager.CheckOutDocument(5, 3);

            IUser faculty = LibrarianDataManager.GetUserById(2);
            Assert.AreEqual(faculty.FirstName, "Sergey");
            Assert.AreEqual(faculty.SecondName, "Afonso");
            Assert.AreEqual(faculty.Phone, "30001");
            Assert.AreEqual(faculty.Type, "Faculty");
            Assert.AreEqual(faculty.Address, "ViaMargutta, 3");
            Copy[] copies = LibrarianDataManager.GetCheckedByUserCopiesList(faculty.CardNumber);
            int c = 0;
            if (copies.Any(copy => copy.DocumentID == 1))
                c++;
            if (copies.Any(copy => copy.DocumentID == 2))
                c++;
            if (copies.Any(copy => copy.DocumentID == 4))            
                c++;            
            Assert.AreEqual(c, 3);

            faculty = LibrarianDataManager.GetUserById(3);
            Assert.AreEqual(faculty.FirstName, "Nadia");
            Assert.AreEqual(faculty.SecondName, "Teixeira");
            Assert.AreEqual(faculty.Phone, "30002");
            Assert.AreEqual(faculty.Type, "Student");
            Assert.AreEqual(faculty.Address, "Via Sacra, 13");
            copies = LibrarianDataManager.GetCheckedByUserCopiesList(faculty.CardNumber);
            c = 0;
            if (copies.Any(copy => copy.DocumentID == 1))
                c++;
            if (copies.Any(copy => copy.DocumentID == 2))
                c++;
            if (copies.Any(copy => copy.DocumentID == 5))
                c++;
            Assert.AreEqual(c, 3);
        }

        [TestMethod]
        public void TestCase8()
        {
            TestCase1();
            PatronDataManager.CheckOutDocument(1, 2);
            PatronDataManager.CheckOutDocument(2, 2);
            PatronDataManager.CheckOutDocument(1, 3);
            PatronDataManager.CheckOutDocument(4, 3);
            Copy copy = LibrarianDataManager.GetCheckedByUserCopiesList(2)[0];
            Assert.AreEqual(DateTime.Parse(copy.ReturningDate).Subtract(DateTime.Now).Days, 27);
            copy = LibrarianDataManager.GetCheckedByUserCopiesList(3)[0];
            Assert.AreEqual(DateTime.Parse(copy.ReturningDate).Subtract(DateTime.Now).Days, 20);
        }

        [TestMethod]
        public void TestCase9()
        {
            TestCase1();            
            Assert.AreEqual(LibrarianDataManager.GetAllCopiesList().Count(copy => copy.DocumentID == 1), 3);
            Assert.AreEqual(LibrarianDataManager.GetAllCopiesList().Count(copy => copy.DocumentID == 2), 2);
            Assert.AreEqual(LibrarianDataManager.GetAllCopiesList().Count(copy => copy.DocumentID == 3), 1);
            Assert.AreEqual(LibrarianDataManager.GetAllAVMaterialsList().Length, 2);
            Assert.AreEqual(LibrarianDataManager.GetAllUsersList().Count(user => user.Type != "Librarian"), 3);
        }
    }
}
