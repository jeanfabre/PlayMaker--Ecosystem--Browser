
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class WebLink
    {
        public string webLinkString = string.Empty;
        public WebLink(string stringToAdd)
        {
            webLinkString = stringToAdd;
        }
    }
}