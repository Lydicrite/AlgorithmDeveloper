using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;
using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl.Styles;
using AlgorithmDeveloper.Resources.UI.Controls.Viewports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Display.Core;
using AlgorithmDeveloper.AlgoDev.Model.Vertices.Graph.Layout;

namespace AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling
{
    public partial class AbstractAlgoController : UserControl
    {
        private AlgoModel? _model = null;

        /// <summary>
        /// Получает или задает модель алгоритма, связанного с этим контроллером.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoModel? Model
        {
            get
            {
                return _model;
            }
            set 
            { 
                if (_model != null)
                    _model.Clear();
                _model = value;
                UpdateVisualization();
            }
        }

        public AbstractAlgoController()
        {
            InitializeComponent();
        }






        /// <summary>
        /// Обновляет визуализацию модели во Viewports.
        /// </summary>
        private void UpdateVisualization()
        {
            if (_model == null || _model.Vertices.Count == 0)
            {
                _viewport.Figures = null;
                return;
            }

            // Размещаем вершины для визуализации
            ArrangeVertices();

            // Передаем вершины и модель во векторный viewport
            _viewport.Figures = _model.Vertices.Where(v => v is not JumpPoint).OfType<IFigure>();
            _viewport.Model = _model;
        }

        /// <summary>
        /// Размещает вершины модели по координатам для визуализации.
        /// Теперь делегирует работу классу алгоритма компоновки.
        /// </summary>
        private void ArrangeVertices()
        {
            if (_model == null)
                return;

            _model.Update();
            // Включаем нормализацию слоёв по несвязанным компонентам; стратегия по умолчанию: shortest-path
            ILayoutAlgorithm layout = new SugiyamaLayoutAlgorithm(
                LayeringStrategy.ShortestPath,
                normalizeComponents: true,
                VisualizationSettings.HorizontalSpacing,
                VisualizationSettings.VerticalSpacing,
                VisualizationSettings.StartX,
                VisualizationSettings.StartY
            );
            _model.ApplyLayout(layout);
        }
    }
}
