using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

const string DEV = @"\\.\ACPIDriver";
const uint IOCTL = 0x9C40A488;

var cache = new { cpu_fan=0, gpu_fan=0, cpu_color="0", gpu_color="0", ok=false };
var lk = new object();

_ = Task.Run(async () => {
    while (true) {
        try {
            using var h = Open();
            while (true) {
                int c = (Read(h,0x0464)<<8)|Read(h,0x0465);
                int g = (Read(h,0x046C)<<8)|Read(h,0x046B);
                if (g < 100) g = 0;
                string cc = c>=4500?"2":c>=3000?"1":"0";
                string gc = g>=4500?"2":g>=3000?"1":"0";
                lock(lk) cache = new { cpu_fan=c, gpu_fan=g, cpu_color=cc, gpu_color=gc, ok=true };
                await Task.Delay(1000);
            }
        } catch {
            lock(lk) cache = cache with { ok=false };
            await Task.Delay(3000);
        }
    }
});

var s = new System.Net.HttpListener();
s.Prefixes.Add("http://localhost:18900/");
s.Start();
Console.WriteLine("OK");

while (true) {
    var c = await s.GetContextAsync();
    c.Response.AddHeader("Access-Control-Allow-Origin","*");
    c.Response.ContentType = "application/json; charset=utf-8";
    if (c.Request.Url!.AbsolutePath is "/fan" or "/") {
        string json; lock(lk) json = $$"""{"cpu_fan":{{cache.cpu_fan}},"gpu_fan":{{cache.gpu_fan}},"cpu_color":"{{cache.cpu_color}}","gpu_color":"{{cache.gpu_color}}","ok":{{(cache.ok?"true":"false")}}}""";
        var b = System.Text.Encoding.UTF8.GetBytes(json);
        c.Response.StatusCode = 200;
        await c.Response.OutputStream.WriteAsync(b);
    } else { c.Response.StatusCode = 404; }
    c.Response.Close();
}

static SafeFileHandle Open() {
    var h = CreateFileW(DEV, 0xC0000000, 3, 0, 3, 0, 0);
    if (h.IsInvalid) throw new Exception("open fail");
    return h;
}
static int Read(SafeFileHandle h, int a) {
    uint i=(uint)a,o=0, r;
    if(!DeviceIoControl(h,IOCTL,ref i,4,ref o,4,out r,0)) throw new Exception("read fail");
    return (int)(o&0xFF);
}

[DllImport("kernel32",SetLastError=true,CharSet=CharSet.Unicode)]
static extern SafeFileHandle CreateFileW(string n,uint a,uint s,int p,uint c,uint f,int t);
[DllImport("kernel32",SetLastError=true)]
static extern bool DeviceIoControl(SafeFileHandle h,uint c,ref uint i,int isz,ref uint o,int osz,out uint r,int _);
