using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            // YOU NEED TO FILL IN HERE
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            var consoleWriterProps = Props.Create<ConsoleWriterActor>();
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "MyConsoleWriter");

            var validatorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
            var validatorActor = MyActorSystem.ActorOf(validatorProps, "MyValidator");

            var consoleReaderProps = Props.Create(() => new ConsoleReaderActor(validatorActor));
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "MyConsoleReader");

            var actorSelection = MyActorSystem.ActorSelection("/");

            // tell console reader to begin
            //YOU NEED TO FILL IN HERE
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.AwaitTermination();
        }
    }
    #endregion
}
