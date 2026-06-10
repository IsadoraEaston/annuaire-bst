using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnuaireBST.Models;

public class BSTNode<T>
{
    public T Value;
    public BSTNode<T> Left;
    public BSTNode<T> Right;
    // Propriété de commodité : est-ce une feuille ?
    public bool IsLeaf => Left == null && Right == null;

    public BSTNode(T value)
    {
        Value = value;
        Left = null;
        Right = null;
    }
}
