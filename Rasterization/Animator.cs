using System.Windows.Threading;

namespace Rasterization;

public class Animator
{
    private DispatcherTimer _timer;
    private float interval;
    private List<Action> _actions = new List<Action>();
    private DateTime  _lastTime;
    
    public Animator( float interval,params Action[]? animations)
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(interval);

        if (animations != null)
            foreach (var action in animations)
            {
                RegisterAnimation(action);
            }

        _timer.Tick += OnTick;
    }
    
    
    public void Start()
    {
        _lastTime = DateTime.Now;
        _timer.Start();
    }
    
    private void OnTick(object sender, EventArgs e)
    {
        foreach (var action in _actions)
        {
            action.Invoke();
        }
    }
    
    public void RegisterAnimation(Action action)
    {
        _actions.Add(action);
    }

    public float GetDeltaTime()
    {
        DateTime currentTime = DateTime.Now;
        float deltaTime = (float) (currentTime - _lastTime).TotalMilliseconds;
        _lastTime = currentTime;
        return deltaTime;
    }
}