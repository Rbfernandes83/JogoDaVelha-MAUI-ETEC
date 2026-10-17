
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MauiAppJogoDaVelha;

public partial class MainPage : ContentPage
{
    private string vez = "X";
    private bool jogoFinalizado = false;
    private int vitoriasX = 0;
    private int vitoriasO = 0;
    private int empates = 0;
    private int jogadas = 0;

    private Button[] casas = Array.Empty<Button>();

    private readonly Color corX = Color.FromArgb("#65E4EF");
    private readonly Color corO = Color.FromArgb("#FFB47B");
    private readonly Color corPadrao = Color.FromArgb("#203453");
    private readonly Color corVitoria = Color.FromArgb("#176B62");

    private readonly int[][] combinacoes =
    {
        new[] { 0, 1, 2 },
        new[] { 3, 4, 5 },
        new[] { 6, 7, 8 },
        new[] { 0, 3, 6 },
        new[] { 1, 4, 7 },
        new[] { 2, 5, 8 },
        new[] { 0, 4, 8 },
        new[] { 2, 4, 6 }
    };

    public MainPage()
    {
        InitializeComponent();

        casas = new[]
        {
            btn00, btn01, btn02,
            btn10, btn11, btn12,
            btn20, btn21, btn22
        };

        AtualizarPlacar();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (jogoFinalizado || sender is not Button botao)
            return;

        if (!string.IsNullOrEmpty(botao.Text))
            return;

        botao.Text = vez;
        botao.TextColor = vez == "X" ? corX : corO;
        botao.IsEnabled = false;

        jogadas++;

        // Animação da jogada
        await botao.ScaleTo(0.85, 90, Easing.CubicOut);
        await botao.ScaleTo(1.0, 140, Easing.SpringOut);

        // Verificar todas as possibilidades de vitória
        foreach (int[] combinacao in combinacoes)
        {
            if (casas[combinacao[0]].Text == vez &&
                casas[combinacao[1]].Text == vez &&
                casas[combinacao[2]].Text == vez)
            {
                jogoFinalizado = true;

                if (vez == "X")
                    vitoriasX++;
                else
                    vitoriasO++;

                lblStatus.Text = "Partida encerrada!";
                lblResultado.Text = $"🏆 Jogador {vez} venceu!";

                AtualizarPlacar();

                foreach (int indice in combinacao)
                {
                    casas[indice].BackgroundColor = corVitoria;
                    casas[indice].BorderColor = corX;

                    await casas[indice].ScaleTo(
                        1.08, 120, Easing.CubicOut);

                    await casas[indice].ScaleTo(
                        1.0, 120, Easing.CubicIn);
                }

                BloquearTabuleiro();
                return;
            }
        }

        // Verificar empate
        if (jogadas == 9)
        {
            jogoFinalizado = true;
            empates++;

            lblStatus.Text = "Partida encerrada!";
            lblResultado.Text = "Empate! Ninguém venceu.";

            AtualizarPlacar();
            BloquearTabuleiro();
            return;
        }

        // Alternar jogadores
        vez = vez == "X" ? "O" : "X";

        lblStatus.Text = $"Vez do jogador {vez}";
        lblStatus.TextColor = vez == "X" ? corX : corO;
    }

    private void AtualizarPlacar()
    {
        lblPlacarX.Text = vitoriasX.ToString();
        lblPlacarO.Text = vitoriasO.ToString();
        lblEmpates.Text = empates.ToString();
    }

    private void BloquearTabuleiro()
    {
        foreach (Button casa in casas)
            casa.IsEnabled = false;
    }

    private async void Reiniciar_Clicked(object sender, EventArgs e)
    {
        jogoFinalizado = true;

        foreach (Button casa in casas)
        {
            casa.Text = "";
            casa.BackgroundColor = corPadrao;
            casa.BorderColor = Color.FromArgb("#405A80");
            casa.Scale = 1;
            casa.IsEnabled = true;
        }

        jogadas = 0;
        vez = "X";

        lblStatus.Text = "Vez do jogador X";
        lblStatus.TextColor = corX;
        lblResultado.Text = "";

        await Tabuleiro.FadeTo(0.35, 120);
        await Tabuleiro.FadeTo(1.0, 250);

        jogoFinalizado = false;
    }
}
