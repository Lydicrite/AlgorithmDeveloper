using System;
using System.Collections.Generic;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout
{
    /// <summary>
    /// ��������� ��� ���������� ���������� ������, ������� ������������� ������� <see cref="AbstractAutomata"/>.
    /// </summary>
    public interface ILayoutAlgorithm
    {
        /// <summary>
        /// ����������� ������� ������ � ������������.
        /// </summary>
        /// <param name="model">������ ���������.</param>
        void Arrange(AbstractAutomata model, bool UseJumpPoints = false);
    }
}