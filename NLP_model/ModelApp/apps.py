from ModelApp.BERT_Model import get_request_results
from django.apps import AppConfig
import threading
import sys
import os

class ModelappConfig(AppConfig):
    name = 'ModelApp'
    model_folder = r"ModelApp\BERT_Model"
    saved_model_path = os.path.join(model_folder, r"saved_models\joint_bert_model")
    input_path = os.path.join(model_folder, r"data\requests.txt")
    output_path = os.path.join(model_folder, r"data\responses.txt")
    
    sys.path.append(model_folder)

    # model_mutex = threading.Lock()

    threading.Thread(target=get_request_results.handle_user_request, 
        args=(saved_model_path, input_path, output_path,),
            daemon=True).start()
    