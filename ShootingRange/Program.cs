using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        enum Statuses : int
        {
            waiting = 1, //ожидание своей очереди
            OnPos,     //стрелок занимает позицию
            ReadyToShoot,     //стрелок готов к стрельбе
            finishedShoot,     //стреок закончил стрельбу
            ApproachesEnd,     // подходы закончились, стрелок покидает очередь

            PreparationAllowed = 10, // инструктор разрешает подготовку к стрельбе
            ShootingAllowed     // инструктор разрешаеть стрелять

        }
        CancellationTokenSource source = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            int[] iSumTimeEvent = new int[5] { 0, 0, 0, 0, 0 }; // массив для хранения сумм времени каждого события (debug), можно просто так же сложить в одну переменную
            int[] iPositions = new int[6] { 0, 0, 0, 0, 0, 0 }; // массив из 6 направлений, где 0 - свободное направление, 1 - занятое стрелком
            int isummtime = 0;
            long timeСompetition = 0;
            Shooter[] Shooters = new Shooter[13];
            for (int i = 0; i < Shooters.Length; i++)            // инициализируем массив стрелков   
            {
                Shooters[i] = new Shooter(i + 1);
            }
            Instructor Beavis = new Instructor(1); // Объявляем инструкторов
            Instructor Butthead = new Instructor(2);



            stopwatch.Start(); //альтернатива второй переменной для подсчета времени
            StartСompetition(ref Shooters, ref Beavis, ref Butthead, ref iPositions, ref iSumTimeEvent);
            stopwatch.Stop(); // затраченное время на мероприятие получим из данных диагностики
            
            // Теперь, когда мы собрали массив с суммарным временем по каждом событию, расчитаем 
             
                isummtime= iSumTimeEvent.Sum();

            timeСompetition = stopwatch.ElapsedMilliseconds / 1000;
            TimeSpan tsSummTimeEvent = TimeSpan.FromSeconds(isummtime);
            TimeSpan tstimeСompetition = TimeSpan.FromSeconds(timeСompetition);

            Console.WriteLine($"Сумма затраченного времени: {tsSummTimeEvent.ToString("mm")}  мин" +
                $" { tsSummTimeEvent.ToString("ss")} сек,  Длительность стрельб: {tstimeСompetition.ToString("mm")}   мин" +
                $"{ tstimeСompetition.ToString("ss")}  сек");
            Console.ReadKey();
        }

        // начинаем состязание!
        static void StartСompetition(ref Shooter[] Shooters, ref Instructor Beavis, ref Instructor Butthead,
                                        ref int[] Positions, ref int[] iSumTimeEvent)
        {
            Shooter ShooterForBeavis = new Shooter(0);
            Shooter ShooterForButthead = new Shooter(0);


            bool first_it = true;  // первая итерация цикла

            bool NotFreePosition; // для выхода из функции по окончанию состязания
            int NextShooter = 1; // переменная для определения следующего стрелка в очереди, по умолчанию 1

            while (true)
            {
                NotFreePosition = false;


                //ищем  позицию для стрелка, который ждет своей очереди, если она найдена, то ищем новую позицию для следующего стрелка,
                //иначе прекращаем поиск, если позиций нет

                foreach (Shooter Shoot in Shooters)
                {
                    // проверяем текущий статус стрелка, если больше 1, проверяем следующего
                    if (Shoot.GetStatus() > 2)
                        continue;
                    if (NextShooter == Shoot.GetNumber())
                    {
                        if (FindFreePositionFirst(ref Positions, Shoot))
                        {
                            if (NextShooter != 13)
                                NextShooter = Shoot.GetNumber() + 1;
                            else
                                NextShooter = 1;
                            continue;
                        }
                        else
                            break;
                    }

                }

                // проверим наличие свободных позиций, если после цикла для поиска направлений не было занято ни одна позиция, значит стрелять больше некому

                if (CheckLockPosition(ref Positions))
                    NotFreePosition = true;
                if (!NotFreePosition)
                    return;

                /* Костыль. Нужен для учета времени первого события по поиску свободных направлений. При последующих итерациях можно выполнять одновременно два события
                 * Поиск свободных направлений и подготовку к стрельбе на занятых направлениях.
                 */
                if (first_it)
                {
                    Thread.Sleep(SumTimeEvent(ref iSumTimeEvent, 0));
                    first_it = false; // сразу переведу бит, чтобы не забыть
                }
                else
                    SumTimeEvent(ref iSumTimeEvent, 0);




                foreach (Shooter Shoot in Shooters)
                {
                    if (Shoot.GetStatus() != 2)
                        continue;

                    if (InstructorOnPosition(ref Beavis, Shoot))
                    {
                        ShooterForBeavis = Shoot;// чтобы больше не бегать по циклам
                        continue;
                    }

                    if (InstructorOnPosition(ref Butthead, Shoot))
                    {
                        ShooterForButthead = Shoot;
                        continue;
                    }


                }

                //Thread.Sleep(4000);
                Thread.Sleep(SumTimeEvent(ref iSumTimeEvent, 1));

                // выводим сообщение от стрелков, которые готовы к стрельбе
                if (ShooterForBeavis.GetStatus() == 3)
                    ShooterForBeavis.PrintStatus(Beavis.iNumber, SetStatusAsString(ShooterForBeavis.GetStatus()));
                if (ShooterForButthead.GetStatus() == 3)
                    ShooterForButthead.PrintStatus(Butthead.iNumber, SetStatusAsString(ShooterForButthead.GetStatus()));

                //
                Thread.Sleep(SumTimeEvent(ref iSumTimeEvent, 2));


                // выводим сообщение от инструкторов на разрешение стрельбы
                if (ShooterForBeavis.GetStatus() == 3)
                    Beavis.PrintStatus(ShooterForBeavis.iNumber, ShooterForBeavis.iPosition, SetStatusAsString(Convert.ToInt32(Statuses.ShootingAllowed)));
                if (ShooterForButthead.GetStatus() == 3)
                    Butthead.PrintStatus(ShooterForButthead.iNumber, ShooterForButthead.iPosition, SetStatusAsString(Convert.ToInt32(Statuses.ShootingAllowed)));


                //

                Thread.Sleep(SumTimeEvent(ref iSumTimeEvent, 3));

                // начало стрельбы 


                CarryingOutShooting(Beavis, ShooterForBeavis, ref Positions);
                CarryingOutShooting(Butthead, ShooterForButthead, ref Positions);

                Thread.Sleep(SumTimeEvent(ref iSumTimeEvent, 4));

                // и сменим позицию инструктора
                NextPosInstructor(ref Positions, ref Beavis, ref Butthead);
            }
        }
        // меняем позиции по кругу с 1 по 6
        static void NextPosInstructor(ref int[] Positions, ref Instructor Beavis, ref Instructor Butthead)
        {
            int BeavisPos = Beavis.GetPosition();
            int ButtheadPos = Butthead.GetPosition();
            if (BeavisPos != 5)
                Beavis.PutPosition(BeavisPos + 2);
            else
                Beavis.PutPosition(1);

            if (ButtheadPos != 6)
                Butthead.PutPosition(ButtheadPos + 2);
            else
                Butthead.PutPosition(2);
        }

        // Поиск свободной позиции для стрелка
        static bool FindFreePositionFirst(ref int[] Positions, Shooter Shoot)
        {
            for (int i = 0; i < Positions.Length; i++)
            {

                if (Positions[i] == 0)
                {

                    Shoot.PutStatus(Convert.ToInt32(Statuses.OnPos));
                    Positions[i] = 1;
                    Shoot.PutPosition(i + 1);
                    Shoot.PrintStatus(-1, SetStatusAsString(Shoot.GetStatus()));
                    return true;
                }


            }
            return false; // если свободных направлений не осталось, нет смысла искать их для последующих стрелков

        }
        // Провека наличия всех свободных мест
        static bool CheckLockPosition(ref int[] Positions)
        {
            
            foreach (int Pos in Positions)
                if (Pos == 1)
                    return true;

            return false;
        }

        static string SetStatusAsString(int Status)
        {
            switch (Status)
            {
                //сообщения для стрелков
                case 2: return "Занял направление!"; 
                case 3: return "К стрельбе готов!"; 
                case 4: return "Стрельбу окончил!"; 
                // для инструкторов
                case 10: return "Подготовиться к стрельбе!"; 
                case 11: return "Произвести стрельбу!"; 
                default: return ""; 
            }
        }
        // проверяем соотвествие позиций инструктора и стрелка, выполняем событие по подготовке к стрельбе
        static bool InstructorOnPosition(ref Instructor Instructor, Shooter Shoot)
        {

            if (Instructor.GetPosition() == -1 | (Instructor.GetPosition() == Shoot.GetPosition()))
            {

                Instructor.PrintStatus(Shoot.iNumber, Shoot.iPosition, SetStatusAsString(Convert.ToInt32(Statuses.PreparationAllowed)));

                if (Instructor.GetPosition() == -1)
                    Instructor.PutPosition(Shoot.iPosition);

                Shoot.PutStatus(Convert.ToInt32(Statuses.ReadyToShoot));
                //  Thread.Sleep(1000);
                // Shoot.PrintStatus(Instructor.iNumber, SetStatusAsString(Shoot.GetStatus()));

                return true;// debug
            }
            return false;// debug
        }


        static bool CarryingOutShooting(Instructor instructor, Shooter Shoot, ref int[] Positions)
        {
            //проверяем соотвествие позиций стрелка и инструктора
            if (instructor.GetPosition() == Shoot.GetPosition())
            {

                //   Thread.Sleep(2000);
                // меняем статус стрелка и сообщаем о выполнении инструкции стрелком
                Shoot.PutStatus(Convert.ToInt32(Statuses.finishedShoot));
                Shoot.Shooting(); // уменьшаем число подходов на 1 при стрельбе
                Shoot.PrintStatus(instructor.iNumber, SetStatusAsString(Shoot.GetStatus()));
                //  Thread.Sleep(2000);
                if (Shoot.GetQuantityApproaches() != 0) // чек оставшихся подходов, если кол-во подходов закончилось, ставим статус об окончании участия стрелка в состязании
                    Shoot.PutStatus(Convert.ToInt32(Statuses.waiting));
                else
                    Shoot.PutStatus(Convert.ToInt32(Statuses.ApproachesEnd));
                Positions[Shoot.iPosition - 1] = 0; // освобождаем направление
                return true; // debug
            }
            return false; // debug
        }

        //здесь собираем суммарное время по каждой операции
        static int SumTimeEvent(ref int[] iSumTimeEvent, int EventNum)
        {
            Random RandomNum = new Random();
            int Delay = 0;
            //в зависимости от текущего события подбираем рандомное число
            switch (EventNum)
            {
               case 0: Delay = RandomNum.Next(3, 10); break; // Занял направление
                case 1: Delay = RandomNum.Next(2, 6); break; // Подготовиться к стрельбе
                case 2: Delay = RandomNum.Next(1, 4); break; // К стрельбе готов
                case 3: Delay = RandomNum.Next(1, 2); break; // Произвести стрельбу
                case 4: Delay = RandomNum.Next(5, 15); break;
                default: Delay = 0; break;// Стрельбу окончил
            }
            iSumTimeEvent[EventNum] += Delay; // число записываем в нужный нам индекс
            return Delay * 1000; // возращаемый параметр передаем умножаем на 1000 мс, чтобы задержка сработала корректно
        }



    }

}



