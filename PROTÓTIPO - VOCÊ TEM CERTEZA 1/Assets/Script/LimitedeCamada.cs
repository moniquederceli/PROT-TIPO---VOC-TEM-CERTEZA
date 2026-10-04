using UnityEngine;

// Este script NÃO faz nada sozinho. Ele é só uma "etiqueta" que você coloca
// em um objeto vazio (LimiteFolhas) dentro do Canvas.
//
// O DragDocument procura por esta etiqueta e, quando uma folha é trazida para
// frente, ela passa por cima das outras folhas, mas NUNCA ultrapassa este
// objeto na lista da Hierarchy. Assim o GrennFilter (e as telas) continuam por cima.
public class LimiteDeCamada : MonoBehaviour
{
}