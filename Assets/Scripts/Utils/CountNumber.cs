
namespace Utils
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Classe para ajudar a contar números, como por exemplo, em placares de pontuação
    /// </summary>
    public class CountNumber
    {
        private float? _easyOutNum_Last = null;

        /// <summary>
        /// conta um número, do início ao fim, no tempo determinado, iniciando rápido e finalizando mais lentamente (EasyOut)
        /// </summary>
        /// <param name="startstep">começa a contar por este número</param>
        /// <param name="endstep">finaliza a contagem neste número</param>
        /// <param name="seconds">quanto tempo demorará pra contar</param>
        /// <returns></returns>
        public IEnumerable<int> EasyOut(float startstep, float endstep, float seconds)
        {
            float t = 0.0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / seconds;
                var xx=Mathf.SmoothStep(0.0f, 1.0f, t);

                var numberToWrite = Mathf.Lerp(startstep, endstep, xx);
                int intnumb = (int)numberToWrite;
                if (_easyOutNum_Last == null || _easyOutNum_Last.ToString() != intnumb.ToString())
                    yield return intnumb;
            }
        }
    }
}
