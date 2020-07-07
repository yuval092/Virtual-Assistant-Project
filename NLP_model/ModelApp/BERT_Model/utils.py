from vectorizers.bert_vectorizer import BERTVectorizer
from models.joint_bert import JointBertModel
from readers.goo_format_reader import Reader
from itertools import chain
from sklearn import metrics
import tensorflow as tf
import argparse
import pickle
import os

model = r"saved_models\joint_bert_model"
