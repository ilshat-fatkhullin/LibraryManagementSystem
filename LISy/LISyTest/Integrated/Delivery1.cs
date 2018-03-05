﻿using System;
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
    public class Delivery1
    {        
        private const long PATRON_1_ID = 2,
            PATRON_2_ID = 3,
            PATRON_3_ID = 4,
            FACULTY_ID = 2,
            STUDENT_ID = 3,
            BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID = 1,
            BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID = 2,
            BOOK_REFERENCE_ID = 3;


        public void Initialize()
        {
            DatabaseDataManager.ClearAll();

            LibrarianDataManager.AddUser(new Librarian("LibrarianName", "LibrarianSurname", "80000000000", "Address"),
                "librarian_1", "12345");
            LibrarianDataManager.AddUser(new Faculty("FacultyName", "FacultySurname", "80000000000", "Address"),
                "patron_1", "12345");
            LibrarianDataManager.AddUser(new Student("StudentName", "StudentSurname", "80000000000", "Address"),
                "patron_2", "12345");
            LibrarianDataManager.AddUser(new Student("StudentName", "StudentSurname", "80000000000", "Address"),
                "patron_3", "12345");

            LibrarianDataManager.AddDocument(new Book("Authors", "Book_1", "Publisher", "Edition", 2018, false, "Keys", "", 100));
            LibrarianDataManager.AddDocument(new Book("Authors", "Book_2", "Publisher", "Edition", 2018, true, "Keys", "", 50));
            LibrarianDataManager.AddDocument(new InnerMaterial("Authors", "Book_3", "Book", "Keys", 1, 1, "https://"));
        }

        /// <summary>
        /// Action: Patron p checks out a copy of book b.
        /// Effect: Librarians can see that Patron p has one copy of book b and there is one copy of book b in the library.
        /// </summary>
        [TestMethod]
        public void TestCase1()
        {            
            PatronDataManager.CheckOutDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);

            Copy[] copies = LibrarianDataManager.GetAllCopiesList();
            bool flag = false;

            foreach (var c in copies)
            {
                if (c.PatronID == PATRON_1_ID && c.DocumentID == BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID)
                {
                    flag = true;
                    break;                    
                }
            }

            PatronDataManager.ReturnDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID); 

            Assert.IsTrue(flag);
        }

        /// <summary>
        /// Action: A patron 'p' checks out book by A.
        /// Effect: The system does not change its state. Maybe a message notifying the patron can read: the library does not have book 'b'
        /// </summary>
        [TestMethod]
        public void TestCase2()
        {
            Copy[] oldCopies = LibrarianDataManager.GetAllCopiesList();
            PatronDataManager.CheckOutDocument(int.MaxValue, PATRON_1_ID);
            Copy[] newCopies = LibrarianDataManager.GetAllCopiesList();
            bool flag = false;
            for (int i = 0; i < oldCopies.Length; i++)            
            {
                if (oldCopies[i].Equals(newCopies[i]))
                {
                    flag = true;
                    break;                    
                }
            }
            Assert.IsFalse(flag);
        }

        /// <summary>
        /// Action: 'f' checks out book 'b'
        /// Effect: The book is checked out by 'f' with returning time of 4 weeks(from the day it was checked out)
        /// </summary>
        [TestMethod]
        public void TestCase3()
        {
            PatronDataManager.CheckOutDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, FACULTY_ID);

            Copy[] copies = LibrarianDataManager.GetAllCopiesList();
            bool flag = false;

            foreach (var c in copies)
            {
                if (c.ReturningDate != null)
                {
                    double days = (DateTime.Parse(c.ReturningDate) - DateTime.Now).TotalDays;

                    if (c.PatronID == FACULTY_ID &&
                        c.DocumentID == BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID &&
                        Math.Abs(days - Book.FACULTY_RETURN_TIME) < 1)
                    {
                        flag = true;
                        break;                        
                    }
                }
            }

            PatronDataManager.ReturnDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, FACULTY_ID);

            Assert.IsTrue(flag);
        }

        /// <summary>
        /// Action: 'f' checks out book 'b'
        /// Effect: The book is checked out by 'f' with returning time of 4 weeks(from the day it was checked out)
        /// </summary>
        [TestMethod]
        public void TestCase4()
        {
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, FACULTY_ID);

            Copy[] copies = LibrarianDataManager.GetAllCopiesList();
            bool flag = false;

            foreach (var c in copies)
            {
                if (c.ReturningDate != null)
                {
                    double days = (DateTime.Parse(c.ReturningDate) - DateTime.Now).TotalDays;

                    if (c.PatronID == FACULTY_ID &&
                        c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID &&
                        Math.Abs(days - Book.FACULTY_RETURN_TIME) < 1)
                    {
                        flag = true;
                        break;                        
                    }
                }
            }

            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, FACULTY_ID);

            Assert.IsTrue(flag);
        }

        /// <summary>
        /// Action: Three patrons try to check out book A.
        /// Effect: Only first two patrons can check out the copy of book A.The third patron sees an empty list of books.
        /// </summary>        
        [TestMethod]
        public void TestCase5()
        {
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_2_ID);
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_3_ID);

            Copy[] copies = LibrarianDataManager.GetAllCopiesList();

            bool checked1 = false, checked2 = false, checked3 = false;

            foreach (Copy c in copies)
            {
                if (c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID)
                {
                    if (c.PatronID == PATRON_1_ID)
                    {
                        checked1 = true;
                    }
                    if (c.PatronID == PATRON_2_ID)
                    {
                        checked2 = true;
                    }
                    if (c.PatronID == PATRON_3_ID)
                    {
                        checked3 = true;
                    }
                }
            }

            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);
            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_2_ID);
            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_3_ID);

            Assert.IsTrue(checked1 && checked2 && !checked3);
        }

        /// <summary>
        /// Action: Patron p tries to check out another copy c' of book b.
        /// Effect: None. In particular, librarians can check that Patron p has the same copy c of book b as before and copy c' is still in the library.
        /// </summary>
        [TestMethod]
        public void TestCase6()
        {
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);

            int count = 0;

            foreach (var c in LibrarianDataManager.GetAllCopiesList())
            {
                if (c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID &&
                    c.PatronID == PATRON_1_ID)
                {
                    count++;
                }
            }

            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);

            Assert.IsTrue(count == 1);
        }

        /// <summary>
        /// Action: p1 and p2 check out b1.
        /// Effect: The system should track both bookings
        /// </summary>
        [TestMethod]
        public void TestCase7()
        {
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);
            PatronDataManager.CheckOutDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_2_ID);

            int size = 0;
            bool first = false, last = false;

            foreach (var c in LibrarianDataManager.GetAllCopiesList())
            {
                if (c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID)
                {
                    size++;
                }
                if (c.PatronID == PATRON_1_ID &&
                    c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID)
                {
                    first = true;
                }
                if (c.PatronID == PATRON_2_ID &&
                    c.DocumentID == BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID)
                {
                    last = true;
                }
            }

            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_1_ID);
            PatronDataManager.ReturnDocument(BOOK_TWO_COPY_BESTSELLER_NO_REFERENCE_ID, PATRON_2_ID);

            Assert.IsTrue(last && first && size == 2);
        }

        /// <summary>
        /// Action: 's' checks out book 'b'
        /// Effect: The book is checked out by 's' with returning time of 3 weeks(from the day it was checked out)
        /// </summary>
        [TestMethod]
        public void TestCase8()
        {
            PatronDataManager.CheckOutDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);

            bool flag = false;

            foreach (var c in LibrarianDataManager.GetAllCopiesList())
            {
                if (c.ReturningDate != null)
                {
                    double days = (DateTime.Parse(c.ReturningDate) - DateTime.Now).TotalDays;

                    if (c.PatronID == STUDENT_ID &&
                        c.DocumentID == BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID &&
                        Math.Abs(days - Book.STUDENT_RETURN_TIME) < 1)
                    {
                        flag = true;
                        return;
                    }
                }
            }

            PatronDataManager.ReturnDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);

            Assert.IsTrue(flag);
        }

        /// <summary>
        /// Action: 's' checks out book 'b'
        /// Effect: The book is checked out by 's' with returning time of 2 weeks(from the day it was checked out)
        /// </summary>        
        [TestMethod]
        public void TestCase9()
        {
            PatronDataManager.CheckOutDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);

            bool flag = false;

            foreach (var c in LibrarianDataManager.GetAllCopiesList())
            {
                if (c.ReturningDate != null)
                {
                    double days = (DateTime.Parse(c.ReturningDate) - DateTime.Now).TotalDays;

                    if (c.PatronID == STUDENT_ID &&
                        c.DocumentID == BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID &&
                        Math.Abs(days - Book.STUDENT_RETURN_TIME) < 1)
                    {
                        flag = true;
                        return;
                    }
                }
            }

            PatronDataManager.ReturnDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);

            Assert.IsTrue(flag);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestCase10()
        {
            PatronDataManager.CheckOutDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);
            PatronDataManager.CheckOutDocument(BOOK_REFERENCE_ID, STUDENT_ID);

            bool flag = false;
            foreach (var c in LibrarianDataManager.GetAllCopiesList())
            {
                if (c.DocumentID == BOOK_REFERENCE_ID && c.PatronID == STUDENT_ID)
                {
                    Assert.IsTrue(false);
                }

                if (c.DocumentID == BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID &&
                    c.PatronID == STUDENT_ID)
                {
                    flag = true;
                }
            }

            PatronDataManager.ReturnDocument(BOOK_ONE_COPY_NOT_BESTSELLER_NO_REFERENCE_ID, STUDENT_ID);            
            Assert.IsTrue(flag);
        }        
    }
}