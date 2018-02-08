namespace Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// classe que ajuda a escrever textos como se estivessem sendo digitados
    /// </summary>
    public class MimicWriting
    {

        /// <summary>
        /// escreve um texto como se estivesse sendo digitado por um humano
        /// </summary>
        /// <param name="textToType">Texto pra ser digitado</param>
        /// <param name="seconds">quanto tempo demorará a digitação</param>
        /// <returns></returns>
        public IEnumerable<string> TypeText(string textToType, float seconds)
        {
            Queue<char> queue = new Queue<char>(textToType.ToCharArray());
            StringBuilder typed = new StringBuilder();
            int lastIntNumb=0;
            float startstep=0f, endstep=(float)queue.Count;
            float t = 0.0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / seconds;
                var xx = Mathf.SmoothStep(0.0f, 1.0f, t);
                var numb = Mathf.Lerp(startstep, endstep, xx);
                int intnumb = (int)numb;
                if ((lastIntNumb==0 || intnumb != lastIntNumb) && queue.Any())
                    yield return typed.Append(queue.Dequeue()).ToString();
            }
        }

    }
}
