using Godot;
using System.Threading.Tasks;


// code from https://www.reddit.com/r/godot/comments/174nfgo/comment/ljz9ygu/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
public partial class PlayerCamera : Camera3D
{
    [Export] private float period = 0.3f;
    [Export] private float magnitude = 0.4f;


   	public async void CameraShakeAsync()
    {
        Transform3D initial_transform = this.Transform;
        float elapsed_time = 0.0f;

        while(elapsed_time < period)
        {
            var offset = new Vector3(
                (float)GD.RandRange(-magnitude, magnitude),
                (float)GD.RandRange(-magnitude, magnitude),
                0.0f
            );

            Transform3D new_transform = initial_transform;
            new_transform.Origin += offset;
            Transform = new_transform;

            elapsed_time += (float)GetProcessDeltaTime();

            GD.Print("Camera Shaked!");

            await ToSignal(GetTree(), "process_frame");
        }

        Transform = initial_transform;
    }
}