using UnityEngine;

public interface IHoverable
{

    /// Вызывается, когда курсор попал на объект.
    void OnMouseEnter();


    /// Вызывается, когда курсор ушёл с объекта.
    void OnMouseExit();
}
