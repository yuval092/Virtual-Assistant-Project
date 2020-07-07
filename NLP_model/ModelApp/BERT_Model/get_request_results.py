from ModelApp.BERT_Model.vectorizers.bert_vectorizer import BERTVectorizer
from ModelApp.BERT_Model.models.joint_bert import JointBertModel
from ModelApp.BERT_Model.readers.goo_format_reader import Reader
# from vectorizers.bert_vectorizer import BERTVectorizer
# from models.joint_bert import JointBertModel
# from readers.goo_format_reader import Reader
from itertools import chain
from sklearn import metrics
import tensorflow as tf
import argparse
import pickle
import numpy
import json
import time
import sys
import os


def get_bert_vectorizer():
    sess = tf.compat.v1.Session()

    bert_model_hub_path = 'https://tfhub.dev/google/bert_uncased_L-12_H-768_A-12/1'
    bert_vectorizer = BERTVectorizer(sess, bert_model_hub_path)

    return sess, bert_vectorizer


def get_model(model_path, sess):
    # loading models
    print('Loading models ...')
    if not os.path.exists(model_path):
        print('Folder `%s` not exist' % model_path)

    with open(os.path.join(model_path, 'tags_vectorizer.pkl'), 'rb') as handle:
        tags_vectorizer = pickle.load(handle)
    with open(os.path.join(model_path, 'intents_label_encoder.pkl'), 'rb') as handle:
        intents_label_encoder = pickle.load(handle)

    return JointBertModel.load(model_path, sess), intents_label_encoder, tags_vectorizer


def get_results(data_text, bert_vectorizer, model, tags_vectorizer,intents_label_encoder):
    input_ids, input_mask, segment_ids, valid_positions, sequence_lengths = bert_vectorizer.transform(
    data_text)

    predicted_tags, predicted_intents = model.predict_slots_intent(
        [input_ids, input_mask, segment_ids, valid_positions],
        tags_vectorizer, intents_label_encoder, remove_start_end=True)
    
    # tf.compat.v1.reset_default_graph()

    try:
        predicted_intents = predicted_intents.tolist()[0]
        predicted_tags = predicted_tags[0]
    except:
        raise Exception("Model Prediction Failed")

    return predicted_intents, predicted_tags


def handle_user_request_loop(in_path, out_path, bert_vectorizer, model, intents_label_encoder, tags_vectorizer):
    while True:
        if os.stat(in_path).st_size == 0:           # if file is empty
            continue

        with open(in_path, 'r') as f:
            request = f.readline()
        with open(in_path, 'w') as f:
            f.write("")                               # delete the request from file
        
        try:
            intent, tags = get_results([request], bert_vectorizer, model, tags_vectorizer, intents_label_encoder)
            result = {
                "intent": intent,
                "slots": tags
            }

        except Exception as e:
            with open(out_path, 'w') as f:
                f.write(json.dumps({"Error": e.args}))
            continue

        with open(out_path, 'w') as f:
            f.write(json.dumps(result))             # write result to file

        time.sleep(0.25)


def handle_user_request(model_path, in_path, out_path):

    try:
        # remove previous requests (to avoid delay)
        with open(in_path, 'w') as f:
            f.write("")
        with open(out_path, 'w') as f:
            f.write("")
        
        sess, bert_vectorizer = get_bert_vectorizer()
        model, intents_label_encoder, tags_vectorizer = get_model(model_path, sess)
        handle_user_request_loop(in_path, out_path, bert_vectorizer, model, intents_label_encoder, tags_vectorizer)
        
    except Exception as e:
            with open(out_path, 'w') as f:
                f.write(json.dumps({"CriticalError": e.args}))