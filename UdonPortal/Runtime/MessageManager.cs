
using UdonSharp;
using UnityEngine;

namespace Nomlas.UdonPortal
{
    public abstract class MessageManager : UdonSharpBehaviour
    {
        public abstract void Message(string message);
    }
}