using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Integral;
using Assets.Scripts.EMath.MathCalculator;
using Assets.Scripts.EMath.MathCalculator.Exceptions;
using Assets.Scripts.EMath.Utils;
using Assets.Scripts.Plotter;
using Assets.Scripts.Plotter.Line;
using Assets.Scripts.Plotter.Plane;
using Assets.Scripts.PlotterHandler;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject functionPanel;
    public GameObject[] functionInputs;
    public GameObject aLimitObject;
    public GameObject bLimitObject;
    public GameObject informationPanel;
    public TextMeshProUGUI[] informationRows;
    public GameObject optionPanel;
    public Slider qualitySlider;
    public Slider zoomSlider;
    public GameObject notificationPanel;
    public GameObject modalPanel;
    public Mesh lineMesh;

    private float quality = 6; // Values between 1 (minQuality) and 6 (maxQuality)
    private float zoom = 10; // Values between 1 (minZoom) and 20 (maxZoom)
    private int rotation = 90;

    private string[] expressions;
    private IFunction[] functions;
    private Axis axis;
    private Optional<double> a;
    private Optional<double> b;

    public IDefiniteIntegrator integrator { get; } = new GaussDefiniteIntegrator();
    private IPlotterHandler plotter;
    private int usedFunctions = 0;
    private bool plotted;
    private bool newMarker;

    public void Plot(IPlotterHandler plotter)
    {
        if (this.plotter == plotter)
        {
            return;
        }

        newMarker = true;
        this.plotter = plotter;

        plotter.Clear();
        functionPanel.SetActive(false);
        CleanFunctionInputs();

        Clear();

        if (!plotted)
        {
            HideNotificationPanel();
            ShowMainPanel();
        }

        ShowFunctionPanel();
    }

    private void Clear()
    {
        axis = Axis.x;
        a = Optional<double>.Empty();
        b = Optional<double>.Empty();
        functions = new IFunction[0];
        expressions = new string[0];
    }

    public void HideNotificationPanel()
    {
        if (!notificationPanel.activeSelf)
        {
            return;
        }

        notificationPanel.SetActive(false);
    }

    public void ShowMainPanel()
    {
        if (mainPanel.activeSelf)
        {
            return;
        }

        mainPanel.SetActive(true);
    }

    public void ShowFunctionPanel()
    {
        if (functionPanel.activeSelf)
        {
            return;
        }

        GetInputField(aLimitObject).text = a.Map<string>(v => v.ToString()).OrElse("");
        GetInputField(bLimitObject).text = b.Map<string>(v => v.ToString()).OrElse("");

        foreach (string e in expressions)
        {
            AddFunction(e);
        }

        if (usedFunctions == 0)
        {
            AddFunction();
        }

        functionPanel.transform.Find("Add Function Button").gameObject.SetActive(plotter.functionLimit > 1);
        functionPanel.transform.Find("Remove Function Button").gameObject.SetActive(plotter.functionLimit > 1);

        Transform axisInput = functionPanel.transform.Find("Axis Input");
        axisInput.gameObject.SetActive(plotter.axisInput);
        axisInput.Find("Slider").GetComponent<Slider>().value = axis == Axis.x ? 1 : 2;

        functionPanel.SetActive(true);
    }

    public void ShowInformationPanel()
    {
        informationPanel.SetActive(true);
    }

    public void ShowOptionPanel()
    {
        qualitySlider.value = quality;
        zoomSlider.value = zoom;

        optionPanel.SetActive(true);
    }

    public void AddFunction(string exp = "")
    {
        if (usedFunctions > plotter.functionLimit - 1)
        {
            ShowInformationModal("Se alcanzó el límite de funciones permitidas para esta gráfica");
            return;
        }

        GetInputField(functionInputs[usedFunctions]).text = exp;
        functionInputs[usedFunctions++].SetActive(true);
    }

    public void RemoveFunction()
    {
        if (usedFunctions == 1)
        {
            ShowErrorModal("Se necesita al menos una función");
            return;
        }

        DeleteLastElement();
    }

    public void BackFunction()
    {
        if (!newMarker)
        {
            functionPanel.SetActive(false);
            CleanFunctionInputs();
        }
        else
        {
            ShowErrorModal("Presiona \"Aceptar\" para procesar los valores introducidos");
        }
    }

    public void BackOption()
    {
        optionPanel.SetActive(false);
    }

    public void BackInformation()
    {
        informationPanel.SetActive(false);
    }

    public void ProcessFunctions()
    {
        if (FunctionInputProcessed())
        {
            functionPanel.SetActive(false);
            SetInformation();
            CleanFunctionInputs();
            plotted = true;
            newMarker = false;
        }
    }

    public void ProcessOptions()
    {
        quality = qualitySlider.value;
        zoom = zoomSlider.value;

        plotter.Plot(axis, functions, a.OrElse(0), b.OrElse(0), quality, rotation, zoom);
        optionPanel.SetActive(false);
    }

    public void ProcessRotation()
    {
        rotation -= 90;

        if (rotation < 0)
        {
            rotation += 360;
        }

        plotter.Plot(axis, functions, a.OrElse(0), b.OrElse(0), quality, rotation, zoom);
    }

    private void CleanFunctionInputs()
    {
        while (usedFunctions > 0)
        {
            DeleteLastElement();
        }
    }

    private void DeleteLastElement()
    {
        functionInputs[--usedFunctions].SetActive(false);
    }

    private bool FunctionInputProcessed()
    {
        try
        {
            string aString = GetInputField(aLimitObject).text;

            if (string.IsNullOrEmpty(aString))
            {
                ShowErrorModal("El límite inferior es obligatorio");
                return false;
            }

            string bString = GetInputField(bLimitObject).text;

            if (string.IsNullOrEmpty(bString))
            {
                ShowErrorModal("El límite superior es obligatorio");
                return false;
            }

            double a = double.Parse(aString);
            double b = double.Parse(bString);

            if (!plotter.negativeLimits && a < 0)
            {
                ShowErrorModal("El límite inferior debe ser mayor o igual a 0");
                return false;
            }

            if (a >= b)
            {
                ShowErrorModal("El límite inferior debe ser menor al límite superior");
                return false;
            }

            string[] exp = new string[usedFunctions];

            for (int i = 0; i < usedFunctions; i++)
            {
                exp[i] = GetInputField(functionInputs[i]).text;

                if (string.IsNullOrEmpty(exp[i]))
                {
                    ShowErrorModal("No se permiten declarar funciones vacías");
                    return false;
                }
            }

            Calculator.Clear();

            float axisValue = functionPanel.transform.Find("Axis Input/Slider").GetComponent<Slider>().value;

            axis = axisValue == 1 ? Axis.x : Axis.y;

            IFunction[] func = GetFunctions(exp);
            plotter.Plot(axis, func, a, b, quality, rotation, zoom);
            expressions = exp;
            this.a = Optional<double>.Of(a);
            this.b = Optional<double>.Of(b);
            functions = func;

            return true;
        }
        catch (Exception exception)
        {
            ShowErrorModal(exception.Message);
        }

        return false;
    }

    private IFunction[] GetFunctions(string[] exp)
    {
        IFunction[] func = new IFunction[exp.Length];

        for (int i = 0; i < func.Length; i++)
        {
            func[i] = new ExpressionFunction(exp[i]);

            if (i > 0 && func[i - 1].independent != func[i].independent)
            {
                throw new InvalidVariableException("Las funciones tienen que declarar la misma variable");
            }
        }

        return func;
    }

    private void SetInformation()
    {
        int length = expressions.Length;

        for (int i = 0; i < length; i++)
        {
            informationRows[i].text = $"Función {i + 1}: {expressions[i]}";
            informationRows[i].color = LineColor.Get(i);
            informationRows[i].gameObject.SetActive(true);
        }

        informationRows[length].text = plotter.GetResultMessage();
        informationRows[length].color = PlaneColor.Get(0);
        informationRows[length].gameObject.SetActive(true);

        for (int i = length + 1; i < informationRows.Length; i++)
        {
            informationRows[i].gameObject.SetActive(false);
        }
    }

    private void ShowErrorModal(string message)
    {
        modalPanel.transform.Find("Fill/Title").GetComponent<TextMeshProUGUI>().text = "Error";
        SetModalMessage(message);
    }

    private void ShowInformationModal(string message)
    {
        modalPanel.transform.Find("Fill/Title").GetComponent<TextMeshProUGUI>().text = "Aviso";
        SetModalMessage(message);
    }

    private void SetModalMessage(string message)
    {
        modalPanel.transform.Find("Fill/Message").GetComponent<TextMeshProUGUI>().text = message;
        modalPanel.SetActive(true);
    }

    public void CloseModal()
    {
        modalPanel.SetActive(false);
    }

    private TMP_InputField GetInputField(GameObject obj)
    {
        return obj.transform.Find("Input").GetComponent<TMP_InputField>();
    }
}
