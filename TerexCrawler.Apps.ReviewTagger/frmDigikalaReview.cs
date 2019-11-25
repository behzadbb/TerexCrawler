using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerexCrawler.Apps.ReviewTagger
{
    public partial class frmDigikalaReview : Form
    {
        public frmDigikalaReview()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string review = "";
            if (!string.IsNullOrEmpty(txtSingleReview.Text))
            {
                review = txtSingleReview.Text.Length > 25 ? txtSingleReview.Text.Substring(0, 25) + " ... " : txtSingleReview.Text;

                var negItem = checkNegative.CheckedItems.OfType<string>().ToList();
                var posItem = checkPositive.CheckedItems.OfType<object>().ToList();

                if (posItem.Any())
                {
                    listBoxAspect.Items.AddRange(posItem.Select(x => review + " " + x + "#pos").ToArray());
                }
                if (negItem.Any())
                {
                    listBoxAspect.Items.AddRange(negItem.Select(x => review + " " + x + "#neg").ToArray());
                }
            }
            clearAspect();
        }
        private void clearAspect()
        {
            txtSingleReview.Text = "";
            foreach (int i in checkNegative.CheckedIndices)
            {
                checkNegative.SetItemCheckState(i, CheckState.Unchecked);
            }
            foreach (int i in checkPositive.CheckedIndices)
            {
                checkPositive.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void frmDigikalaReview_Load(object sender, EventArgs e)
        {
            txtReview.Text = @"خب بعد حدودا 2 ماه کار با گوشی میتونم بگم که بینهایت زیباست ... خیلی صفحه نمایش فوق العاده ای داره که اگه بک راندای با کیفیت بزارید خیلی خیلی بهتون کیف میده کار با گوشی .... بسیار روون و کار باهاش بینهایت سادست به طوری که اصلا دیگه سخته با ایفون دکمه دار کار کرد .. وقتی موزیک با گوشی پخش میشه واقعا صدای بلند و با کیفیتی داره .... دوربینشم واقعا عالیه اللبته در حد دوربینای DSLR حرفه ای نیست ولی کاملا یه سرو گردن از دوربینای گوشی های تلفن بالاتره .....کاملا خوش دست نیست و برای اینکه از باتری قدرتمندش کاملا رضایت داشته باشید باید خیلی چیزارو خاموش کنید ... مثلا چی پی اس و بک راند ریفرش و .... اینکه نمیتونی هرچیزی که میخواییم راحت دانلود کنی و تو نرم افزار دلخواهت پخشش کنی آزاردهنده بوده و هست .
در کل خیلی از خریدم راضیم و به نظرم بعد 3 4 سال کار با ایفون 6 و الان که ایفون 10 ... کاملا به سیستم عامل IOS عادت کردم و خیلی از کار باهاش لذت میبرم ..
0
2
".Trim();
        }

        private void txtSingleReview_MouseClick(object sender, MouseEventArgs e)
        {
            if (Clipboard.GetText() != null && !string.IsNullOrEmpty(Clipboard.GetText()))
            {
                txtSingleReview.Text = Clipboard.GetText().Trim();
                Clipboard.Clear();
            }

        }

        private void txtSingleReview_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
