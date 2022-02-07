using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Instructor
    {

        public int iNumber;
        //public bool InstructorFree; // не использую
        public int iInstructorPosition;
        public Instructor(int Number)
        {
            this.iNumber = Number;
            // this.InstructorFree = true;  // 1 - свободен, 0 - занят
            this.iInstructorPosition = -1;
        }

        /* public void SetIntsructorFree( bool InstructorFree)
         {
             this.InstructorFree = InstructorFree;
         }
         public bool IsIntsructorFree()
         {
             return this.InstructorFree;
         }
        */
        public int GetNumber()
        {
            return this.iNumber;
        }
        public void PutPosition(int iPos)
        {
            this.iInstructorPosition = iPos;
        }
        public int GetPosition()
        {
            return this.iInstructorPosition;
        }
        public void PrintStatus(int NumShoot, int Pos, string message)
        {
            Console.WriteLine($"Направление: {Pos}, инструктор:  {this.iNumber}, стрелок {NumShoot}: " + message);
        }
    }
}
