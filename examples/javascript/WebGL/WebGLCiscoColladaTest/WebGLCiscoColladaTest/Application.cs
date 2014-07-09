using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebGLCiscoColladaTest;
using WebGLCiscoColladaTest.Design;
using WebGLCiscoColladaTest.HTML.Pages;

namespace WebGLCiscoColladaTest
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application : ApplicationWebService
    {
        public IHTMLCanvas canvas;

        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2013/201311/20131110-dae

            var oo = new List<THREE.Object3D>();

            #region scene
            var window = Native.window;

            var camera = new THREE.PerspectiveCamera(
                45,
                window.aspect,
                1,
                1000
                //2000
                );
            camera.position.z = 400;

            // scene

            var scene = new THREE.Scene();

            var ambient = new THREE.AmbientLight(0x101030);
            scene.add(ambient);

            var directionalLight = new THREE.DirectionalLight(0xffeedd);
            directionalLight.position.set(0, 0, 1);
            scene.add(directionalLight);

            // WebGLRenderer preserveDrawingBuffer 
            var renderer = new THREE.WebGLRenderer(

                new
                {
                    preserveDrawingBuffer = true
                }
            );

            // https://github.com/mrdoob/three.js/issues/3836
            renderer.setClearColor(0xfffff, 1);

            renderer.setSize(window.Width, window.Height);

            renderer.domElement.AttachToDocument();
            renderer.domElement.style.SetLocation(0, 0);

            this.canvas = (IHTMLCanvas)renderer.domElement;


            var mouseX = 0;
            var mouseY = 0;
            var st = new Stopwatch();
            st.Start();

            Native.window.onframe +=
                delegate
                {
                    renderer.clear();


                    //camera.aspect = window.aspect;
                    //camera.aspect = canvas.clientWidth / (double)canvas.clientHeight;
                    //camera.aspect = canvas.aspect;
                    camera.updateProjectionMatrix();


                    oo.WithEach(
                        x =>
                            x.rotation.y = st.ElapsedMilliseconds * 0.0001
                    );


                    camera.position.x += (mouseX - camera.position.x) * .05;
                    camera.position.y += (-mouseY - camera.position.y) * .05;

                    camera.lookAt(scene.position);

                    renderer.render(scene, camera);


                };

            Native.window.onresize +=
                delegate
                {
                    if (canvas.parentNode == Native.document.body)
                    {
                        renderer.setSize(window.Width, window.Height);
                    }

                };
            #endregion

            new Cisco().Source.Task.ContinueWithResult(
                dae =>
                {
                    //dae.scale.x = 30;
                    //dae.scale.y = 30;
                    //dae.scale.z = 30;

                    //dae.castShadow = true;
                    //dae.receiveShadow = true;

                    dae.scale.x = 5;
                    dae.scale.y = 5;
                    dae.scale.z = 5;

                    dae.position.y = -80;

                    scene.add(dae);
                    oo.Add(dae);


                }
            );
        }


    }

    [Obsolete("jsc should generate this")]
    class Cisco : THREE_ColladaAsset
    {

        //---------------------------
        //daeimporter
        //---------------------------
        //Missing Textures-
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal-02.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Aluminum_Anodized.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Corrogated_Shiny.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Panel.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Rough.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Rusted.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Metal_Seamed.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\SUP720-01.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\Supervisor-template.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\_7609-RSP-01.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\_7609-RSP-02.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\__Fencing_Diamond_Mesh__copy12_copy12_copy_copy.png
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\a48-port-10-100.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\b-Side-Plates-03.jpg
        //X:\jsc.svn\examples\javascript\WebGL\WebGLCiscoColladaTest\WebGLCiscoColladaTest\web\assets\WebGLCiscoColladaTest\cisco\brass1_copy.jpg

        //---------------------------
        //OK   
        //---------------------------


        //Uncaught TypeError: Cannot read property 'input' of null 
        // 		var vertexData = sources[ this.vertices.input['POSITION'].source ].data;
        // geometry: "ID2035"

        public Cisco()
            : base(
                // Embedded Resource
                //"assets/WebGLCiscoColladaTest/cisco.dae"
                "assets/WebGLCiscoColladaTest/cisco2.dae"
                )
        {

        }
    }



    public class THREE_ColladaAsset
    {
        public readonly TaskCompletionSource<THREE.Object3D> Source = new TaskCompletionSource<THREE.Object3D>();

        public THREE_ColladaAsset(string uri)
        {
            var loader = new THREE.ColladaLoader();

            loader.options.convertUpAxis = true;

            // this does NOT work correctly?
            //loader.options.centerGeometry = true;

            loader.load(

                uri,

                new Action<THREE.ColladaLoaderResult>(
                    collada =>
                    {
                        var dae = collada.scene;


                        ////o.position.y = -80;
                        //scene.add(dae);
                        //oo.Add(dae);

                        //dae.scale = new THREE.Vector3(5, 5, 5);

                        this.Source.SetResult(dae);

                    }
                )
            );
        }
    }
}
