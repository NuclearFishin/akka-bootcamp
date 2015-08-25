using Akka.Actor;
using System.IO;

namespace WinTail
{
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("Input was blank, Please try again"));

                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUrl(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess($"Starting processing for {msg}"));

                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError($"{msg} is not an existing URI on disk"));

                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
        }

        private bool IsFileUrl(string path)
        {
            return File.Exists(path);
        }
    }
}
