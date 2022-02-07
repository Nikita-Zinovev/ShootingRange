using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Shooter
    {
        public int iNumber;
        public int iShooterStatus;
        public int iQuantityApproaches;
        public int iPosition;

        public Shooter(int iNumber, int iShootStatus = 1, int iQuantityApproaches = 5)  // по умолчанию статус ожидает в очереди (1) и  кол-во подходов 5
        {
            this.iNumber = iNumber;
            this.iShooterStatus = iShootStatus;
            this.iQuantityApproaches = iQuantityApproaches;
            this.iPosition = -1; // по умолчанию нет позиции 

        }
        public int GetNumber()
        {
            return this.iNumber;
        }
        public int GetStatus()
        {
            return this.iShooterStatus;
        }
        public int GetQuantityApproaches()
        {
            return iQuantityApproaches;
        }
        public void PutStatus(int iShooterStatus)
        {
            this.iShooterStatus = iShooterStatus;
        }
        public void PutPosition(int iPos)
        {
            this.iPosition = iPos;
        }
        public int GetPosition()
        {
            return this.iPosition;
        }

        public void PrintStatus(int iNumberIstructor, string message)
        {

            if (iNumberIstructor == -1) // если стрелок только занял позицию, инструктора там может не быть, поэтому не пишем его
                Console.WriteLine($"Направление: {iPosition}, инструктор:  , стрелок {this.iNumber}: " + message);
            else

                Console.WriteLine($"Направление: {iPosition}, инструктор:  {iNumberIstructor}, стрелок {this.iNumber}: " + message);
        }

        public void Shooting()
        {
            this.iQuantityApproaches -= 1;
        }
    }
}
