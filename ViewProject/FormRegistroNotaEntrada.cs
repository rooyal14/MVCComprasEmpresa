using ControllerProject;
using ModelProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewProject
{
    public partial class FormRegistroNotaEntrada : Form
    {

        private NotaEntradaController notaEntradaController;
        private FornecedorController fornecedorController;
        private ProdutoController produtoController;

        NotaEntrada notaAtual;

        public FormRegistroNotaEntrada(NotaEntradaController notaEntradaController, 
            FornecedorController fornecedorController, 
            ProdutoController produtoController)
        {
            this.notaEntradaController = notaEntradaController;
            this.fornecedorController = fornecedorController;
            this.produtoController = produtoController;
            InitializeComponent();
            InicializaComboBoxs();
        }

        private void InicializaComboBoxs()
        {
            cbxFornecedor.Items.Clear();
            cbxProduto.Items.Clear();
            foreach (Fornecedor fornecedor in
            this.fornecedorController.GetAll())
            {
                cbxFornecedor.Items.Add(fornecedor);
            }
            foreach (Produto produto in
            this.produtoController.GetAll())
            {
                cbxProduto.Items.Add(produto);
            }
        }

        private void btnNovoNota_Click(object sender, EventArgs e)
        {
            ClearControlsNota();
        }

        private void ClearControlsNota()
        {
            dgvNotaEntrada.ClearSelection();
            dgvProdutos.ClearSelection();
            fmIdNota.Text = string.Empty;
            cbxFornecedor.SelectedIndex = -1;
            fmNumero.Text = string.Empty;
            dataEmissao.Value = DateTime.Now;
            dataEntrada.Value = DateTime.Now;
            cbxFornecedor.Focus();
        }

        private void btnGravarNota_Click(object sender, EventArgs e)
        {
            var notaEntrada = new NotaEntrada()
            {
                Id = (fmIdNota.Text == string.Empty ? Guid.NewGuid() :
                    new Guid(fmIdNota.Text)),
                DataEmissao = dataEmissao.Value,
                DataEntrada = dataEntrada.Value,
                FornecedorNota = (Fornecedor)cbxFornecedor.SelectedItem,
                Numero = fmNumero.Text
            };
            notaEntrada = (fmIdNota.Text == string.Empty ? this.notaEntradaController.Insert(
            notaEntrada) : this.notaEntradaController.Update(notaEntrada));
            dgvNotaEntrada.DataSource = null;
            dgvNotaEntrada.DataSource = this.notaEntradaController.GetAll();
            ClearControlsNota();
        }

        private void btnCancelarNota_Click(object sender, EventArgs e)
        {
            ClearControlsNota();
        }

        private void btnRemoverNota_Click(object sender, EventArgs e)
        {
            if (fmIdNota.Text == string.Empty)
            {
                MessageBox.Show(
                "Selecione a NOTA a ser removida no GRID");
            }
            else
            {
                this.notaEntradaController.Remove(
                new NotaEntrada()
                {
                    Id = new Guid(fmIdNota.Text)
                }
                );
                dgvNotaEntrada.DataSource = null;
                dgvNotaEntrada.DataSource =
                this.notaEntradaController.GetAll();
                ClearControlsNota();
            }

        }

        private void dgvNotaEntrada_SelectionChanged(object sender, EventArgs e)
        {
            
            try
            {
                this.notaAtual = this.notaEntradaController.GetNotaEntradaById((Guid)dgvNotaEntrada.CurrentRow.Cells[0].Value);
                fmIdNota.Text = notaAtual.Id.ToString();
                fmNumero.Text = notaAtual.Numero;
                cbxFornecedor.SelectedItem = notaAtual.FornecedorNota;
                dataEmissao.Value = notaAtual.DataEmissao;
                dataEntrada.Value = notaAtual.DataEntrada;
                UpdateProdutosGrid();
            }
            catch (Exception ex)
            {
                this.notaAtual = new NotaEntrada();
            }
        }

        private void UpdateProdutosGrid()
        {
            dgvProdutos.DataSource = null;
            if (this.notaAtual.Produtos.Count > 0)
            {
                dgvProdutos.DataSource = this.notaAtual.
                Produtos;
            }
        }

        private void btnNovoProduto_Click(object sender, EventArgs e)
        {
            ClearControlsProduto();
            if (fmIdNota.Text == string.Empty)
            {
                MessageBox.Show("Selecione a NOTA, que terá " +
                "inserção de produtos, no GRID");
            }
            else
            {
                ChangeStatusOfControls(true);
            }
        }

        private void ChangeStatusOfControls(bool newStatus)
        {
            cbxProduto.Enabled = newStatus;
            fmCusto.Enabled = newStatus;
            fmQuantidade.Enabled = newStatus;
            btnNovoProduto.Enabled = !newStatus;
            btnGravarProduto.Enabled = newStatus;
            btnCancelarProduto.Enabled = newStatus;
            btnRemoverProduto.Enabled = newStatus;
        }

        private void ClearControlsProduto()
        {
            dgvProdutos.ClearSelection();
            fmIdProduto.Text = string.Empty;
            fmCusto.Text = string.Empty;
            fmQuantidade.Text = string.Empty;

        }

        private void btnGravarProduto_Click(object sender, EventArgs e)
        {
            var produtoNota = new ProdutoNotaEntrada()
            {
                Id = (fmIdProduto.Text == string.Empty ? Guid.NewGuid() : new Guid(fmIdProduto.Text)),
                PrecoCustoCompra = Convert.ToDouble(fmCusto.Text),
                ProdutoNota = (Produto)cbxProduto.SelectedItem,
                QuantidadeComprada = Convert.ToDouble(fmQuantidade.Text)
            };
            this.notaAtual.RegistrarProduto(produtoNota);
            this.notaAtual = this.notaEntradaController.Update(
            this.notaAtual);
            ChangeStatusOfControls(false);
            UpdateProdutosGrid();
            ClearControlsProduto();

        }

        private void btnCancelarProduto_Click(object sender, EventArgs e)
        {
            ClearControlsProduto();
            ChangeStatusOfControls(false);
        }
    }
}
