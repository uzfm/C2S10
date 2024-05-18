using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf;
using Google.Protobuf.Collections;
using static Tensorflow.Binding;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Tensorflow;
using Tensorflow.Data;
using Tensorflow.Keras;
using Tensorflow.Util;
using Tensorflow.IO;


using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;

using System.Collections.Generic;
using Tensorflow;
using Tensorflow.Keras;
using static Tensorflow.Binding;

using static Tensorflow.image_ops_impl;


using Tensorflow.Keras.Engine;
using System.Drawing;
//using Numpy;
//using SharpCV;

using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Threading.Tasks;
//using Numpy.Models;
//using NumSharp.Backends;
//using NumSharp;
using Tensorflow.Util;
//using NumSharp;

using Tensorflow.Train;
using Tensorflow;
using Tensorflow.Keras;
using System.Drawing.Imaging;
//using NumSharp;


using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

using Tensorflow.Keras.Utils;
//using Tensorflow.NumPy;
//using Numpy;
//using NumSharp.Core;
//using NumSharp;
using Tensorflow.NumPy;
//using SharpCV;
//using NumSharp;
//using Tensorflow.NumPy;

//using NumSharp;



using Tensorflow.Keras.Models;


using Tensorflow.Keras;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Models;


/// <summary>
/// /////////////////////////////////////////////////////////////////// ML BITMAP  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

using Mat = Emgu.CV.Mat;


namespace MVision
{
    static public class MLD{
        static public string    Path   { get; set; }  // шлях для Моделі
        static public string    PathSamples { get; set; }  // шлях для sepls
        static public string [] Names        { get; set; }  // назви відів семплів (назва папки)


// Кольори у форматі RGB
        static public Color[] NameColor = {
    Color.FromArgb(255, 0, 0), // Червоний
    Color.FromArgb(0, 255, 0), // Зелений
    Color.FromArgb(0, 0, 255), // Синій
      Color.FromArgb(0, 0, 0), // Чорний
    Color.FromArgb(255, 0, 255), // Фіолетовий
    Color.FromArgb(0, 255, 255), // Блакитний
    Color.FromArgb(128, 0, 0), // Бордовий
    Color.FromArgb(0, 128, 0), // Темно-зелений
    Color.FromArgb(0, 0, 128), // Темно-синій
    Color.FromArgb(128, 128, 0), // Оливковий
    Color.FromArgb(128, 0, 128), // Пурпурний
    Color.FromArgb(0, 128, 128), // Бірюзовий
    Color.FromArgb(255, 128, 0), // Помаранчевий
    Color.FromArgb(255, 255, 128), // Жовто-гарячий
    Color.FromArgb(128, 255, 128), // М'ятний
    Color.FromArgb(128, 255, 255), // Світло-блакитний
    Color.FromArgb(128, 128, 128), // Сірий
    Color.FromArgb(192, 192, 192), // Світло-сірий
    Color.FromArgb(64, 64, 64), // Темно-сірий
    Color.FromArgb(255, 255, 0), // Жовтий
};




        public  static int NameIdx(string args)
        {
            // Перевірка на null, щоб уникнути помилки, якщо масив Names ще не ініціалізовано
            if (MLD.Names != null)
            {
                // Знаходимо індекс рядка "Good" в масиві Names
                int index = Array.IndexOf(MLD.Names, args);

                // Перевіряємо, чи знайдено рядок "Good"
                if (index != -1)
                {
                    return index;
                }
                else
                {
                    Console.WriteLine("String " + args + "not found in array name of the samples :" + index);
                }
            }
            else
            {
                Console.WriteLine("The Names array is uninitialized.");
            }
            return 0;
        }




    }



    class  ML_Net  {


    


        // <SnippetDeclareGlobalVariables>
        // static string _assetsPath = SAV.DT.TF_DT.PathAssets;
        // static string _imagesFolder = Path.Combine(_assetsPath, SAV.DT.TF_DT.PathDataSet);
        // static ResaltRgst Resalt = new ResaltRgst();
        // static InMemoryImageData imageToPredict;
        //  public static bool LoadedModelPath = true;


        /// <summary>
        /// ///////////////////////////////////////////////
        /// </summary>

        int batch_size = 64;
        int epochs = 10;
        Shape img_dim = (64, 64);
        IDatasetV2 train_ds, val_ds;
       Model model;



        public void Main()
        {

            PrepareData();
            //BuildModel();
             ResNetModel();
              //  Fast32();
            LoadModel();

        }



        public void BuildModel(){
           
            int num_classes   =  MLD.Names.Length;
      var layers = keras.layers;
          
            var normalization_layer = KerasApi.keras.layers.Rescaling(1.0f / 255);



            model = keras.Sequential(new List<ILayer>{
         layers.Rescaling(1.0f / 255, input_shape:(img_dim.dims[0], img_dim.dims[1], 3)),
           layers.Conv2D(8, kernel_size: (3, 3), padding: "same", activation: keras.activations. Relu),
           layers. MaxPooling2D(),
           layers.Conv2D(16, kernel_size: (3, 3), padding: "same", activation: keras.activations.Relu),
           layers.MaxPooling2D(),
           layers.Conv2D(32, kernel_size: (3, 3), padding: "same", activation: keras.activations.Relu),
           layers.MaxPooling2D(),
           layers.Conv2D(64, kernel_size: (3, 3), padding: "same", activation: keras.activations.Relu),
           layers.MaxPooling2D(),
          layers.Flatten(),
           //layers. Dropout(0.5f),
           layers. Dense(128, activation: keras.activations.Relu),
           layers. Dense(num_classes)
        });

            model.compile(optimizer: keras.optimizers.Adam(),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                metrics: new[] { "accuracy" }
                //experimental_run_tf_function: true

                );

            model.summary();
        }


        public Model ResNetModel()
        {
            int num_classes = MLD.Names.Length;
            var layers = keras.layers;
            var inputLayer = layers.Input(shape: (img_dim.dims[0], img_dim.dims[1], 3), name: "img");
            var input = layers.Rescaling(1.0f / 255).Apply(inputLayer);


            /*      ось пояснення параметрів для методу Conv2D:

          filters: Кількість вихідних фільтрів у згортці.У вашому випадку ви використовуєте 64 фільтри.Це визначає кількість "каналів" або "глибину" вихідного тензора.
          kernel_size: Розмір вікна згортки.У вас вікно розміром 6x6.Це визначає просторові розміри вікна згортки, яке буде "скользити" по вхідному зображенню.
          strides: Крок згортки. Вказує, на скільки пікселів вікно згортки переміщується при кожному кроці.У вас вказаний крок 2x2.
          padding: Цей параметр вказує, як слід додавати "падінг" до вхідного тензора перед згорткою. "same" означає, що до вхідного тензора буде додано такий "падінг", щоб вихідний тензор мав такий же просторовий розмір, як і вхідний.
          activation: Функція активації, яка буде застосована до виходу згорткового шару. У вашому випадку використовується ReLU(rectified linear unit).
          use_bias: Цей параметр вказує, чи слід додавати зсув до виходу згортки.Зазвичай він встановлений у true, що означає, що до кожного вихідного каналу буде додано свій власний зсув.
          groups: Кількість груп, на які розділяються вхідні та вихідні канали. Значення 1 означає, що використовується звичайна згортка, а не групова згортка.        */
            var x1 = layers.Conv2D(
                       filters: 64,
                       kernel_size: new Shape(6, 6),
                       strides: new Shape(2, 2),
                       padding: "same",
                       activation: keras.activations.Relu,
                       use_bias: true,
                       groups: 1).Apply(input);
            x1 = layers.BatchNormalization().Apply(x1);



            var x2 = layers.Conv2D(
            filters: 64,
            kernel_size: new Shape(2, 2),
            strides: new Shape(2, 2),
            padding: "same",
            activation: keras.activations.Relu,
            use_bias: true,
            groups: 1).Apply(input);

            x2 = layers.BatchNormalization().Apply(x2);


            var concatenated = layers.Concatenate().Apply(new Tensors(x1, x2));

            // Додаємо залишковий блок
            var x = layers.Add().Apply(new Tensors(x1, x2));
            x = layers.MaxPooling2D((3, 3), strides: (2, 2), padding: "same").Apply(x);



            int[] Strides = { 64, 128, 128, 128, 256 };

            for (int i = 0; i < 5; i++)
            {
                x = CreateResBlock(x, Strides[i], 2);
            }




            Tensors CreateResBlock(Tensors InputX, int filters, int blocks)
            {

                var layers = keras.layers;
                var y = InputX;


                for (int i = 0; i < blocks; i++)
                {

                    y = layers.Conv2D(filters, (3, 3), padding: "same", activation: keras.activations.Relu).Apply(InputX);
                    y = layers.BatchNormalization().Apply(y);

                    y = layers.Conv2D(filters, (3, 3), padding: "same", activation: keras.activations.Relu).Apply(InputX);
                    y = layers.BatchNormalization().Apply(y);

                    if (i == 0 && x.shape[-1] != filters)
                    {

                        InputX = layers.Conv2D(filters, kernel_size: (1, 1), padding: "same", activation: keras.activations.Relu).Apply(x);
                    }

                    // Додаємо залишковий блок
                    y = layers.Add().Apply(new Tensors(InputX, y));

                }

                return y;
            }


            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(128, activation: keras.activations.Relu).Apply(x);
            x = layers.Dropout(0.2f).Apply(x);



            var outputs = layers.Dense(num_classes, activation: keras.activations.Softmax).Apply(x);



            model = (Model)keras.Model(inputs: inputLayer, outputs: outputs, name: "ResNet50");

            model.summary();



            float newLearningRate = 1e-4f;  // Вкажіть бажану швидкість навчання тут 1e-4f;

            model.compile(
                optimizer: keras.optimizers.RMSprop(newLearningRate),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: false),
                metrics: new[] { "accuracy" }
            );
            return (Model)model;
        }

       public Model Fast32()
        {

            var layers = keras.layers;
            //Gry

            // Створюємо модель за допомогою TensorFlow.Keras.Sequential
            model = keras.Sequential(new List<ILayer> {
    // Нормалізація пікселів
    layers.Rescaling(1.0f / 255, input_shape: (img_dim.dims[0], img_dim.dims[1], 3)),
    // Перший шар згорткової мережі з 32 фільтрами та розміром ядра 3х3
    layers.Conv2D(32, 3, padding: "same", activation: keras.activations.Relu),
    // Шар пулінгу, що зменшує розмірність зображення в 2 рази
    layers.MaxPooling2D(),
    // Другий шар згорткової мережі з 64 фільтрами та розміром ядра 3х3
    layers.Conv2D(64, 3, padding: "same", activation: keras.activations.Relu),
    // Шар пулінгу
    layers.MaxPooling2D(),
     // Другий шар згорткової мережі з 64 фільтрами та розміром ядра 3х3
    layers.Conv2D(64, 2, padding: "same", activation: keras.activations.Relu),
    // Шар пулінгу
    layers.MaxPooling2D(),
    // Другий шар згорткової мережі з 64 фільтрами та розміром ядра 3х3
    layers.Conv2D(64, 2, padding: "same", activation: keras.activations.Relu),
    // Шар пулінгу
    layers.MaxPooling2D(),


    // Розгладжуємо отриманий тензор
    layers.Flatten(),
    // Випадково відключаємо 50% нейронів
    layers.Dropout(0.5f),
    // Повнозв'язний шар з 256 нейронами та функцією активації ReLU
    layers.Dense(256, activation: keras.activations.Relu),
    // Вихідний шар з кількістю нейронів, рівною кількості класів
    layers.Dense(MLD.Names.Length)

               });



            model.compile(optimizer: keras.optimizers.RMSprop(1e-6f),
            loss: keras.losses.SparseCategoricalCrossentropy(from_logits: false),
            metrics: new[] { "accuracy" });

            // Обчислення ваг класів
            model.summary();
            return model;


        }


        public void LoadModel(){
            string classFile = Path.Combine(MLD.Path, "SAMPLES.h5");

            model.load_weights(classFile);


        }






        public void PrepareData()
        {
       

     
            string data_dir = MLD.PathSamples;
            string[] ddd = new string[2];
            ddd[0] = data_dir;
            ddd[1] = data_dir;



            // convert to tensor
               train_ds = KerasApi.keras.preprocessing.image_dataset_from_directory(data_dir,
                validation_split: 0.2f,
                subset: "training",
                //color_mode: "grayscale",
                color_mode: "rgb",
                seed: 123,
                image_size: img_dim,
                batch_size: batch_size );


            val_ds = KerasApi.keras.preprocessing.image_dataset_from_directory(data_dir,
            validation_split: 0.2f,
            subset: "validation",
            //color_mode: "grayscale",
            color_mode: "rgb",
            seed: 123,
            image_size: img_dim,
            batch_size: batch_size);
          
            train_ds = train_ds.shuffle(1000).prefetch(buffer_size: -1);
            val_ds = val_ds.prefetch(buffer_size: -1);
        }





        public void PredictImage(List<Mat> Imgs)
        {
            Stopwatch watch;
            watch = Stopwatch.StartNew();

            var images = new List<Tensor>();

            Shape shape = (1, img_dim[0], img_dim[1], 3);

            foreach (var mat in Imgs)
            {

                var matCopy = mat.ToImage<Rgb, byte>().Resize((int)img_dim.dims[0], (int)img_dim.dims[0], Emgu.CV.CvEnum.Inter.Linear);

                CvInvoke.CvtColor(matCopy, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);

                byte[] dataset = mat.GetRawData();
                float[] floatImageDataFloat = dataset.Select(b => (float)b).ToArray();

                images.Add(tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, shape, name: "Const"));
            }


            var imageTe = tf.stack(images);
            var path_ds = tf.data.Dataset.from_tensor_slices(imageTe);


            var value = model.predict(path_ds);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            var numpyArray = value[0].numpy();
            var class_index = np.argmax(numpyArray[0]);

            Console.WriteLine(/*class_index.ToString() */ "Prediction:" + numpyArray.ToString() + "---");
            Console.WriteLine(/*class_index.ToString() */ "Prediction:" + class_index.ToString() + "------" + elapsedMs.ToString() + " ms");


        }


        public PDT[]  PredictImage(Image<Bgr, byte>[] ImgM, Image<Bgr, byte>[] ImgS){

            //Stopwatch watch;
            //watch = Stopwatch.StartNew();
            List<Tensor> images = new List<Tensor>();
            Shape shape = (1, img_dim[0], img_dim[1], 3);
            PDT PDT = new PDT();

            foreach (var Img in ImgM){
                if (Img == null) { break; }
                var matCopy = Img.Resize((int)img_dim.dims[0], (int)img_dim.dims[0], Emgu.CV.CvEnum.Inter.Linear);
                CvInvoke.CvtColor(matCopy, matCopy, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);
                byte[] dataset = matCopy.Bytes;
                float[] floatImageDataFloat = dataset.Select(b => (float)b).ToArray();
                var constant = tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, shape, name: "Const");
                images.Add(constant);
            }

            foreach (var Img in ImgS){
                if (Img == null) { break; }
                var matCopy = Img.Resize((int)img_dim.dims[0], (int)img_dim.dims[0], Emgu.CV.CvEnum.Inter.Linear);
                CvInvoke.CvtColor(matCopy, matCopy, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);
                byte[] dataset = matCopy.Bytes;
                float[] floatImageDataFloat = dataset.Select(b => (float)b).ToArray();
                var constant = tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, shape, name: "Const");
                images.Add(constant);
            }





            var imageTe = tf.stack(images);
            var path_ds = tf.data.Dataset.from_tensor_slices(imageTe);
            Tensor predictions = model.predict(path_ds, batch_size: 32);
       
            PDT[] DTs = new PDT[predictions.numpy().Count()];

            int idx = 0;

            foreach (var pred in predictions.numpy()){
                DTs[idx] = new PDT();
                var class_index = np.argmax(pred);
                var class_indexs = pred.astype(TF_DataType.TF_FLOAT);
                DTs[idx].ID = (class_index[0].ToByteArray()[0]);
                DTs[idx].Value = new float[class_indexs.Count()];
                int idxs = 0;
                foreach (var val in class_indexs){ DTs[idx].Value[idxs++] = val;}
                DTs[idx].Nema = MLD.Names[DTs[idx].ID];
                idx++;
                //DTs[idx++].ID++;
            }

            return DTs;
        }


     public   void Stop_GPU() {

            keras.backend.clear_session();
           
           // model.

        }

        public PDT PredictImage(Mat maT)
        {    PDT PDT = new PDT();

            Stopwatch watch;
            watch = Stopwatch.StartNew();
            Mat mat = new Mat();
            CvInvoke.Resize(maT,mat , new Size((int)img_dim.dims[0], (int)img_dim.dims[0]));

            // Convert the image to RGB format if it is not already in that format.
            //CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);


            float[] floatImageDataFloat = mat.GetRawData().Select(b => (float)b).ToArray();
            Shape ShapeS = new Shape(1, (int)img_dim.dims[0], (int)img_dim.dims[0], 3);
            var TensorByte = tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, ShapeS);
            var input = tf.expand_dims(TensorByte, 0);
            var path_ds = tf.data.Dataset.from_tensor_slices(input);
            //var path_ds = tf.data.Dataset.from_tensor_slices(tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, ShapeS));
            //path_ds = path_ds.shuffle(32 * 8, seed: 123);
            // path_ds = path_ds.batch(32);

            Tensor predictions = model.predict(path_ds, batch_size: 32);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            //-0.6509194] false
            //-0.6893591 true

            // Перетворити результати передбачення в NumPy array
            var numpyArray = predictions.numpy();
            // Обчислити softmax на виході з моделі
            //var class_index =  tf.nn.softmax(numpyArray[0]);

            var class_index = np.argmax(numpyArray[0]);
            ////var rfrfg=numpyArray[0].numpy();

            Console.WriteLine(class_index.ToString() + "   Prediction took: -- " + elapsedMs.ToString() + " ms  --" + " ++++++++++++" + numpyArray[0].ToString()); ;

            return PDT.SoftMax(predictions);
        }


        public PDT PredictImage(string PashImage) { 
            //   Convert the image to a Mat object
            PDT PDT = new PDT();
            Mat mat = CvInvoke.Imread(PashImage);
           CvInvoke.Resize(mat, mat, new Size((int)img_dim.dims[0], (int)img_dim.dims[0]));
            // Convert the image to RGB format if it is not already in that format.
            CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb); 
            float[] floatImageDataFloat = mat.GetRawData().Select(b => (float)b).ToArray();
            Shape ShapeS = new Shape(1, (int)img_dim.dims[0], (int)img_dim.dims[0], 3);
            var TensorByte = tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, ShapeS);
            var input = tf.expand_dims(TensorByte, 0);
            var path_ds = tf.data.Dataset.from_tensor_slices(input);
            Tensor predictions = model.predict(path_ds, batch_size: batch_size);
            // Перетворити результати передбачення в NumPy array
            var numpyArray = predictions.numpy();
            // Обчислити softmax на виході з моделі
            var class_index = np.argmax(numpyArray[0]);
            return PDT.SoftMax(predictions);
        }



        public class PDT
        {

            public float[] Value;
            public byte ID;
            public string Nema;

            public PDT SoftMax(Tensor value)
            {
                PDT predict = new PDT();
                var numpyArray = value.numpy();
                var rbhr = value.buffer;

                var class_index = np.argmax(numpyArray[0]);
                var class_indexs = numpyArray[0].astype(TF_DataType.TF_FLOAT);

                predict.ID = (class_index[0].ToByteArray()[0]);
                predict.Value = new float[class_indexs.Count()];
                int idx = 0;

                foreach (var val in class_indexs)
                {
                    predict.Value[idx++] = val;
                }

                predict.Nema = MLD.Names[predict.ID];
                predict.ID++;
                return predict;
            }


        }




        public class InMemoryImageData
        {
            public InMemoryImageData(byte[] image, string label, string imageFileName)
            {
                Image = image;
                Label = label;
                ImageFileName = imageFileName;
            }

            public readonly byte[] Image;

            public readonly string Label;

            public readonly string ImageFileName;
        }












    }




    class ML_Nets
    {
        // <SnippetDeclareGlobalVariables>
        // static string _assetsPath = SAV.DT.TF_DT.PathAssets;
        // static string _imagesFolder = Path.Combine(_assetsPath, SAV.DT.TF_DT.PathDataSet);
        // static ResaltRgst Resalt = new ResaltRgst();
        // static InMemoryImageData imageToPredict;
        //  public static bool LoadedModelPath = true;


        /// <summary>
        /// ///////////////////////////////////////////////
        /// </summary>

        int batch_size = 64;
        int epochs = 10;
        Shape img_dim = (64, 64);
        IDatasetV2 train_ds, val_ds;
        Model model;



        public void Main()
        {

            //PrepareData();
            BuildModel();
            LoadModel();

        }



        public void BuildModel()
        {


            int num_classes = MLD.Names.Length;


            var normalization_layer = KerasApi.keras.layers.Rescaling(1.0f / 255);
            var layers = keras.layers;
            model = keras.Sequential(new List<ILayer>{
       layers.Rescaling(1.0f / 255, input_shape:(img_dim.dims[0], img_dim.dims[1], 3)),



                layers.Conv2D(16, 3, padding: "same", activation: keras.activations.Relu),
                layers.MaxPooling2D(),
                layers.Conv2D(32, 3, padding: "same", activation: keras.activations.Relu),
                layers.MaxPooling2D(),
                layers.Conv2D(64, 3, padding: "same", activation: keras.activations.Relu),
                layers.MaxPooling2D(),
                layers.Flatten(),
                layers.Dense(128, activation: keras.activations.Relu),
                layers.Dense(num_classes)

        });

            model.compile(optimizer: keras.optimizers.Adam(),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                metrics: new[] { "accuracy" }
                //experimental_run_tf_function: true

                );

            model.summary();

        }


        public void LoadModel(){
            string classFile = Path.Combine(MLD.Path, "SAMPLES.h5");

            //model.fit(train_ds, validation_data: val_ds, epochs: epochs);

            //model.save_weights(classFile, save_format: "h5");
            //"F:\\DataSet\\WB_NL_C1_New_Test\\my_model.h5"
            model.load_weights(classFile);

        //D:\\DataSet\\WB_NL_C1_New_Test\\Camera 1.h5
        }






        public void PrepareData()
        {



            string data_dir = MLD.PathSamples;
            string[] ddd = new string[2];
            ddd[0] = data_dir;
            ddd[1] = data_dir;



            // convert to tensor
            train_ds = KerasApi.keras.preprocessing.image_dataset_from_directory(data_dir,
             validation_split: 0.2f,
             subset: "training",
             //color_mode: "grayscale",
             color_mode: "rgb",
             seed: 123,
             image_size: img_dim,
             batch_size: batch_size);


            val_ds = KerasApi.keras.preprocessing.image_dataset_from_directory(data_dir,
            validation_split: 0.2f,
            subset: "validation",
            //color_mode: "grayscale",
            color_mode: "rgb",
            seed: 123,
            image_size: img_dim,
            batch_size: batch_size);

            train_ds = train_ds.shuffle(1000).prefetch(buffer_size: -1);
            val_ds = val_ds.prefetch(buffer_size: -1);
        }





        public void PredictImages(List<Mat> Imgs)
        {
            Stopwatch watch;
            watch = Stopwatch.StartNew();

            var images = new List<Tensor>();

            Shape shape = (1, img_dim[0], img_dim[1], 3);

            foreach (var mat in Imgs)
            {

                var matCopy = mat.ToImage<Rgb, byte>().Resize((int)img_dim.dims[0], (int)img_dim.dims[0], Emgu.CV.CvEnum.Inter.Linear);

                CvInvoke.CvtColor(matCopy, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);

                byte[] dataset = mat.GetRawData();
                float[] floatImageDataFloat = dataset.Select(b => (float)b).ToArray();

                images.Add(tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, shape, name: "Const"));
            }


            var imageTe = tf.stack(images);
            var path_ds = tf.data.Dataset.from_tensor_slices(imageTe);


            var value = model.predict(path_ds);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            var numpyArray = value[0].numpy();
            var class_index = np.argmax(numpyArray[0]);

            Console.WriteLine(/*class_index.ToString() */ "Prediction:" + numpyArray.ToString() + "---");
            Console.WriteLine(/*class_index.ToString() */ "Prediction:" + class_index.ToString() + "------" + elapsedMs.ToString() + " ms");


        }




        public PDT PredictImage(Mat mat)
        {
            PDT PDT = new PDT();

            Stopwatch watch;
            watch = Stopwatch.StartNew();

            CvInvoke.Resize(mat, mat, new Size((int)img_dim.dims[0], (int)img_dim.dims[0]));

            // Convert the image to RGB format if it is not already in that format.
            CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgb);
         

            float[] floatImageDataFloat = mat.GetRawData().Select(b => (float)b).ToArray();
            Shape ShapeS = new Shape(1, (int)img_dim.dims[0], (int)img_dim.dims[0], 3);
            var TensorByte = tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, ShapeS);
            var input = tf.expand_dims(TensorByte, 0);
            var path_ds = tf.data.Dataset.from_tensor_slices(input);
            //var path_ds = tf.data.Dataset.from_tensor_slices(tf.constant(floatImageDataFloat, TF_DataType.TF_FLOAT, ShapeS));
            //path_ds = path_ds.shuffle(32 * 8, seed: 123);
            // path_ds = path_ds.batch(32);

            Tensor predictions = model.predict(path_ds, batch_size: 32);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            //-0.6509194] false
            //-0.6893591 true

            // Перетворити результати передбачення в NumPy array
            var numpyArray = predictions.numpy();
            // Обчислити softmax на виході з моделі
            //var class_index =  tf.nn.softmax(numpyArray[0]);

            var class_index = np.argmax(numpyArray[0]);
            ////var rfrfg=numpyArray[0].numpy();

            Console.WriteLine(class_index.ToString() + "   Prediction took: -- " + elapsedMs.ToString() + " ms  --" + " ++++++++++++" + numpyArray[0].ToString()); ;

            return PDT.SoftMax(predictions);
        }






        public class PDT
        {

            public float[] Value;
            public byte ID;
            public string Nema;


            public PDT SoftMax(Tensor value)
            {

                PDT predict = new PDT();

                var numpyArray = value.numpy();
                var rbhr = value.buffer;

                var class_index = np.argmax(numpyArray[0]);
                var class_indexs = numpyArray[0].astype(TF_DataType.TF_FLOAT);




                predict.ID = (class_index[0].ToByteArray()[0]);
                predict.Value = new float[class_indexs.Count()];
                int idx = 0;
                foreach (var val in class_indexs)
                {
                    predict.Value[idx++] = val;
                }

                predict.Nema = MLD.Names[predict.ID];
                predict.ID++;

                return predict;
            }

            public PDT[] SoftMax(Tensor[] values)
            {

                PDT[] predict = new PDT[values.Length];

                int i = 0;
                foreach (var value in values)
                {
                    var numpyArray = value.numpy();
                    var class_index = np.argmax(numpyArray[0]);
                    var class_indexs = numpyArray[0].astype(TF_DataType.TF_DOUBLE);

                    // predict[i].Value = numpyArray.ToString();
                    predict[i].ID = (class_index[0].ToByteArray()[0]);

                    predict[i].Value = new float[class_indexs.Count()];
                    int idx = 0;
                    foreach (var val in class_indexs)
                    {
                        predict[i].Value[idx++] = val;
                    }

                    predict[i].Nema = MLD.Names[predict[i].ID];
                    predict[i].ID++;
                    i++;
                }

                return predict;
            }

        }




        public class InMemoryImageData
        {
            public InMemoryImageData(byte[] image, string label, string imageFileName)
            {
                Image = image;
                Label = label;
                ImageFileName = imageFileName;
            }

            public readonly byte[] Image;

            public readonly string Label;

            public readonly string ImageFileName;
        }












    }




}
