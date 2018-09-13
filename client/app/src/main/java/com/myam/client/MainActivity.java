package com.myam.client;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class MainActivity extends AppCompatActivity {

    private static TextView textResponse;
    private EditText mEditTextMensaje;
    private Button buttonConnect;
    private String message = "Hi client!";
    public static String ip = "192.168.1.79";
    public static int puerto = 5678;

    Socket socket = null;
    private String mensajeEnviado = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mEditTextMensaje = (EditText) findViewById(R.id.editText1);
        buttonConnect = (Button) findViewById(R.id.button1);
        textResponse = (TextView) findViewById(R.id.response);


        send();
    }


    private void send() {

        buttonConnect.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mensajeEnviado = mEditTextMensaje.getText().toString();
                new conectar().execute();
            }
        });
    }

    int i = 0;

    private class conectar extends AsyncTask<Void, Void, Void> {
        String mensaje = null;

        @Override
        protected Void doInBackground(Void... voids) {
            try {

                socket = new Socket(ip, puerto);
                PrintWriter writer = new PrintWriter(socket.getOutputStream());
                writer.print(mensajeEnviado);
                writer.flush();
                socket.shutdownOutput();
                mensaje = ("# " + i + "- Mensaje enviado: " + mensajeEnviado);
                /* ************ esperando respuesta ************** */
               /* socket.getInputStream().available();
                BufferedReader r = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                StringBuilder total = new StringBuilder();
                String line;
                while ((line = r.readLine()) != null) {
                    total.append(line);
                }
                textResponse.setText("Respuesta: " + total.toString());
                /* ****************************** */
                socket.close();
                writer.close();
            } catch (Exception e) {
                e.printStackTrace();
                mensaje = ("Error! " + e.toString());
            }
//finalizo envio
            i++;
            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            textResponse.setText(mensaje);
            super.onPostExecute(aVoid);
        }
    }
}
