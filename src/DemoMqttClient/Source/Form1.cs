using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Standard.Web.Mqtt;

namespace MqttClientDemo
{
	public partial class Form1 : Form
	{
		MqttClient client = null;
		private Dictionary<ushort, string> subscribeInflight = new Dictionary<ushort, string>();
		private Dictionary<ushort, string> unsubscribeInflight = new Dictionary<ushort, string>();

		public Form1()
		{
			InitializeComponent();
			listBox1.Items.Add("Click on the 'Connect' button now to hook up with test.mosquitto.org:1883");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (button1.Text == "Connect")
			{
				if (client == null)
					client = new MqttClient("test.mosquitto.org");
				listBox1.Items.Add("=============================================");

				client.PublishReceived += MqttMsgPubRecv;
				client.Subscribed += MqttMsgSubscribed;
				client.Unsubscribed += MqttMsgUnsubscribed;
				client.Connect();

				if (client.IsConnected)
				{
					listBox1.Items.Add("Connection established.");
					listBox1.Items.Add("Create a topic now and subscribe to it!");
				}
				else
				{
					listBox1.Items.Add("Connection failed. No internet?");
					return;
				}

				button1.Text = "Disconnect";
				button3.Enabled = true;
			}
			else if (button1.Text == "Disconnect")
			{
				client.Close();
				client.Disconnect();
				client = null;
				button1.Text = "Connect";
				button2.Enabled = false;
				button3.Enabled = false;
				button4.Enabled = false;
			}
		}

		private void MqttMsgPubRecv(object sender, MqttPublishEventArgs e)
		{
			if (listBox1.InvokeRequired)
			{
				listBox1.Invoke(new EventHandler(delegate
				{
					listBox1.Items.Add(string.Format("RECV from topic {0}: {1}", e.Topic, Encoding.UTF8.GetString(e.Message)));
				}));
			}
			else
			{
				listBox1.Items.Add(string.Format("RECV from topic {0}: {1}", e.Topic, Encoding.UTF8.GetString(e.Message)));
			}
		}

		private void MqttMsgUnsubscribed(object sender, MqttUnsubscribeAcknowledgeEventArgs e)
		{
			if (listBox1.InvokeRequired)
			{
				listBox1.Invoke(new EventHandler(delegate
				{
					listBox1.Items.Add(string.Format("You are now unsubscribed from topic #{0} [#{1}]", unsubscribeInflight[e.MessageId], e.MessageId));
				}));
			}
			else
			{
				listBox1.Items.Add(string.Format("You are now unsubscribed from topic #{0} [#{1}]", unsubscribeInflight[e.MessageId], e.MessageId));
			}

			if (listBox2.InvokeRequired)
			{
				listBox2.Invoke(new EventHandler(delegate
				{
					listBox2.Items.Remove(unsubscribeInflight[e.MessageId]);
				}));
			}
			else
			{
				listBox2.Items.Remove(unsubscribeInflight[e.MessageId]);
			}

			unsubscribeInflight.Remove(e.MessageId);
		}

		private void MqttMsgSubscribed(object sender, MqttSubscribeAcknowledgeEventArgs e)
		{
			if (listBox1.InvokeRequired)
			{
				listBox1.Invoke(new EventHandler(delegate
				{
					listBox1.Items.Add(string.Format("You are now subscribed to topic {0} [#{1}]", subscribeInflight[e.MessageId], e.MessageId));
				}));
			}
			else
			{
				listBox1.Items.Add(string.Format("You are now subscribed to topic {0} [#{1}]", subscribeInflight[e.MessageId], e.MessageId));
			}

			if (listBox2.InvokeRequired)
			{
				listBox2.Invoke(new EventHandler(delegate
				{
					listBox2.Items.Add(subscribeInflight[e.MessageId]);
				}));
			}
			else
			{
				listBox2.Items.Add(subscribeInflight[e.MessageId]);
			}

			subscribeInflight.Remove(e.MessageId);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string topic = (string)listBox2.Items[listBox2.SelectedIndex];
			listBox1.Items.Add(string.Format("Publishing to topic {0}: {1}", topic, textBox1.Text));
			client.Publish(topic, Encoding.UTF8.GetBytes(textBox1.Text), QosLevel.ExactlyOnce, false);
			textBox1.Text = string.Empty;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox1.Text))
				return;

			if (string.IsNullOrWhiteSpace(textBox1.Text))
				return;

			ushort topicid = client.Subscribe(textBox1.Text, QosLevel.AtLeastOnce);
			subscribeInflight.Add(topicid, textBox1.Text);

			textBox1.Text = string.Empty;
			button2.Enabled = true;
			button4.Enabled = true;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string topic = (string)listBox2.Items[listBox2.SelectedIndex];
			ushort msgid = client.Unsubscribe(topic);
			listBox1.Items.Add(string.Format("Unsubscribing from topic #{0}: {1}", msgid, topic));
			unsubscribeInflight.Add(msgid, topic);
		}
	}
}
